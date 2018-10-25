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
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private TextConverter _textConverter;

        public ImagesController(TextConverter textConverter)
        {
            _textConverter = textConverter;
        }

        [HttpPost]
        [RequestSizeLimit(valueCountLimit: 21474836)]
        // e.g. 2ish mb request limit
        public async Task<ActionResult> Post(string base64Data)
        {
            var base64 = Request.Form["base64Data"];
            var bytes = System.Convert.FromBase64String(base64);
            //var path = @"c:\temp\saved.jpg";
            //if (System.IO.File.Exists(path))
            //{
            //    System.IO.File.Delete(path);
            //}
            //System.IO.File.WriteAllBytes(path, bytes);
            //var magic = new TextConverter();
            var text = await _textConverter.ConvertText(bytes);
            return Ok(text);
        }

        [Route("url")]
        [HttpPost]
        [RequestSizeLimit(valueCountLimit: 21474836)]
        // e.g. 2ish mb request limit
        public async Task<ActionResult> PostUrl([FromForm] string url)
        {
            //var base64 = Request.Form["base64Data"];
            //var bytes = System.Convert.FromBase64String(base64);
            //var path = @"c:\temp\saved.jpg";
            //if (System.IO.File.Exists(path))
            //{
            //    System.IO.File.Delete(path);
            //}
            //System.IO.File.WriteAllBytes(path, bytes);
            //var magic = new TextConverter();
            url = Request.Form["url"];
            var text = await _textConverter.SubmitImageUrlForProcessing(url);
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
    }






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
