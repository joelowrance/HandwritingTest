using System.Diagnostics;
using CanvasToTextSpike.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace CanvasToTextSpike.Controllers
{
    public class HomeController : Controller
    {
        private IOptions<AzureApi> options;

        public HomeController(IOptions<AzureApi> options)
        {
            this.options = options; 
        }

        public IActionResult Index()
        {
            ViewBag.Key = "a secret"; //options.Value.Key;
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
