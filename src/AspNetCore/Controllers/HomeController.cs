using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Controllers
{
    public class HomeController : Controller2Garin
    {
        // для доступа из моделей
        public readonly ILogger<HomeController> Logger;

        public HomeController(ILogger<HomeController> logger)
        {
            Logger = logger;
            Logger.LogInformation("Home constructor", new object[0]);
        }

        public IActionResult Index(PageIM im)
        {
            Logger.LogInformation("Home index", new object[0]);
            return im.ToActionResult(this);
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

        public IActionResult Error()
        {
            return View();
        }
    }
}
