using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


//TODO:
//3.  Upload image and in-memory and then ML
//4.  Upload drawn image.


namespace CanvasToTextSpike.Controllers
{
    public class ParentDTO
    {
        public string Base64Data { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        //[HttpPost]
        //public ActionResult Post()
        //{
        //}

        [HttpPost]
        [RequestSizeLimit(valueCountLimit: 21474836)]
        // e.g. 2ish mb request limit
        public async Task<ActionResult> Post(string base64Data)
        {
            var base64 = Request.Form["base64Data"];
            var bytes = System.Convert.FromBase64String(base64);
            var path = @"c:\temp\saved.jpg";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            System.IO.File.WriteAllBytes(path, bytes);
            var magic = new TextConverter();
            var text = await magic.ConvertText(bytes);
            return Ok(text);
        }

        //[HttpPost]
        //public async Task<ActionResult> Post(IFormFile file)
        //{
        //    if (file == null)
        //    {
        //        return Ok();
        //    }

        //    using (var ms = new MemoryStream())
        //    {
        //        await file.CopyToAsync(ms);
        //        var bytes = ms.ToArray();
        //        var magic = new TextConverter();
        //        var text = await magic.ConvertText(bytes);
        //        return Ok(text);
        //    }
        //}


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok("Empty get");
        //    var client = new HttpClient();
        //    var qs = HttpUtility.ParseQueryString(string.Empty);
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "b069c6d90b464b0f926253668a832dab");
        //    qs["handwriting"] = "true";
        //    var uri = "https://eastus.api.cognitive.microsoft.com/vision/v1.0/recognizeText?" + qs;

        //    var body = Newtonsoft.Json.JsonConvert.SerializeObject(new
        //    {
        //        url =
        //            "https://media.npr.org/assets/img/2016/04/17/handwritten-note_wide-941ca37f3638dca912c8b9efda05ee9fefbf3147.jpg?s=1400"
        //    });
        //    var bytes = Encoding.UTF8.GetBytes(body);
        //    HttpResponseMessage response;
        //    var operationId = "";

        //    using (var content = new ByteArrayContent(bytes))
        //    {
        //        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        //        response = await client.PostAsync(uri, content);

        //        var header = response.Headers.FirstOrDefault(x => x.Key == "apim-request-id");
        //        operationId = header.Value.First();
        //    }


        //    var client2 = new HttpClient();
        //    var queryString = HttpUtility.ParseQueryString(string.Empty);
        //    // Request headers
        //    client2.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "b069c6d90b464b0f926253668a832dab");
        //    var uri2 = $"https://eastus.api.cognitive.microsoft.com/vision/v1.0/textOperations/{operationId}?" + queryString;
        //    var response2 = await client.GetAsync(uri2);
        


            

            


            ///return Ok(response2);

        }

    }


    //using System;
    //using System.Net.Http.Headers;
    //using System.Text;
    //using System.Net.Http;
    //using System.Web;

    //namespace CSHttpClientSample
    //{
    //    static class Program
    //    {
    //        static void Msain()
    //        {
    //            MakeRequest();
    //            Console.WriteLine("Hit ENTER to exit...");
    //            Console.ReadLine();
    //        }

    //        static async void MakeRequest()
    //        {
    //            var client = new HttpClient();
    //            var queryString = HttpUtility.ParseQueryString(string.Empty);

    //            // Request headers
    //            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

    //            // Request parameters
    //            queryString["handwriting"] = "true";
    //            var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/recognizeText?" + queryString;

    //            HttpResponseMessage response;

    //            // Request body
    //            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

    //            using (var content = new ByteArrayContent(byteData))
    //            {
    //                content.Headers.ContentType = new MediaTypeHeaderValue("< your content type, i.e. application/json >");
    //                response = await client.PostAsync(uri, content);
    //            }

    //        }
    //    }
    //}



    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequestSizeLimitAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
    {
        private readonly FormOptions _formOptions;

        public RequestSizeLimitAttribute(int valueCountLimit)
        {
            _formOptions = new FormOptions()
            {
                // tip: you can use different arguments to set each properties instead of single argument
                KeyLengthLimit = valueCountLimit,
                ValueCountLimit = valueCountLimit,
                ValueLengthLimit = valueCountLimit

                // uncomment this line below if you want to set multipart body limit too
                // MultipartBodyLengthLimit = valueCountLimit
            };
        }

        public int Order { get; set; }

        // taken from /a/38396065
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var contextFeatures = context.HttpContext.Features;
            var formFeature = contextFeatures.Get<IFormFeature>();

            if (formFeature == null || formFeature.Form == null)
            {
                // Setting length limit when the form request is not yet being read
                contextFeatures.Set<IFormFeature>(new FormFeature(context.HttpContext.Request, _formOptions));
            }
        }
    }
}
