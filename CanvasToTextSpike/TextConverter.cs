using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CanvasToTextSpike.Controllers;
using Microsoft.Extensions.Options;
// ReSharper disable ClassNeverInstantiated.Local

namespace CanvasToTextSpike
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class TextConverter
    {
        private readonly IOptions<AzureApi> _configuration;
        private readonly HttpClient _httpClient;

        public TextConverter(IOptions<AzureApi> configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration.Value.Key);
        }

        public async Task<string> ConvertText(byte[] image)
        {
            var location = await SubmitImageForProcessing(image);
            var text = await ReadProcessingResult(location);
            return text;
        }

        public async Task<string> ConvertText(string imagePath)
        {
            var bytes = File.ReadAllBytes(imagePath);
            return await ConvertText(bytes);
        }



        private async Task<string> SubmitImageForProcessing(byte[] image)
        {
            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["handwriting"] = true.ToString();
            qs["mode"] = "handwritten";
            var uri = _configuration.Value.Url + "recognizeText?" + qs; 

            using (var content = new ByteArrayContent(image))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                var response = await _httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return response.Headers.GetValues("Operation-Location").FirstOrDefault();
                }

                return response.ReasonPhrase;
            }
        }

        public async Task<string> SubmitImageUrlForProcessing(string url)
        {
            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["handwriting"] = true.ToString();
            qs["mode"] = "Handwritten";
            var uri = _configuration.Value.Url + "recognizeText?" + qs;

            var f = new
            {
                url = url
            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(f));
            //using (var content = new ByteArrayContent(f))
            //{
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await _httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var location = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                    var text = await ReadProcessingResult(location);
                    return text;

            }

            return response.ReasonPhrase;
            //}
        }

        private async Task<string> ReadProcessingResult(string locationUrl)
        {
            await Task.Delay(1500);
            var response = await _httpClient.GetAsync(locationUrl);
            var json = await response.Content.ReadAsStringAsync();
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<MicrosoftHandwritingResult>(json);

            var message = "No text";

            foreach (var line in root.recognitionResult.lines ?? new List<MicrosoftHandwritingResult.Line>())
            {
                message += line.text + Environment.NewLine;
            }
            
            return message;
        }


        //JSON that is returned matches this class structure
        private class MicrosoftHandwritingResult
        {
            public string status { get; set; }
            public RecognitionResult recognitionResult { get; set; }

            public class Word
            {
                public List<int> boundingBox { get; set; }
                public string text { get; set; }
            }

            public class Line
            {
                public List<int> boundingBox { get; set; }
                public string text { get; set; }
                public List<Word> words { get; set; }
            }

            public class RecognitionResult
            {
                public List<Line> lines { get; set; }
            }
        }
    }
}