using System.Diagnostics;
using CanvasToTextSpike.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace CanvasToTextSpike.Controllers
{
    public class AzureApi
    {
        public string Key { get; set; }
        public string Url { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly TextConverter _converter;


        public HomeController(TextConverter converter)
        {
            _converter = converter;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
