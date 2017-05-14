using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using Microsoft.Extensions.Logging;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace AspNetCore.Controllers
{
    public class HomeController : Controller2Garin
    {

        public HomeController(IControllerSettings settings) : base(settings)
        {
            Logger.LogTrace("Home constructor");
        }

        public IActionResult Index(PageIM im)
        {
            Logger.LogTrace("Home index");
            return im.ToActionResult(this);
        }

        public IActionResult Test()
        {
            Logger.LogTrace("Home test");
            return View();
        }
    }
}
