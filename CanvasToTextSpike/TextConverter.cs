using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace CanvasToTextSpike
{
    public class TextConverter
    {
        private HttpClient _httpClient;
        private const string key = "b069c6d90b464b0f926253668a832dab"; //TODO: move to config
        public TextConverter()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
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
            bool handwriting = false;
            qs["handwriting"] = handwriting.ToString();
            var uri = "https://eastus.api.cognitive.microsoft.com/vision/v1.0/recognizeText?" + qs; //TODO:  set root, move to config


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

        private async Task<string> ReadProcessingResult(string locationUrl)
        {
            await Task.Delay(2500);
            var response = await _httpClient.GetAsync(locationUrl);
            var json = await response.Content.ReadAsStringAsync();
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<MicrosoftHandwritingResult>(json);
            var message = root.recognitionResult.lines.Select(x => x.text)
                .Aggregate((acc, cur) => acc += cur + System.Environment.NewLine);
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