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

        public HomeController(IStorage storage, ILoggerFactory loggerFactory, IStringLocalizerFactory localizerFactory) : base(storage, loggerFactory)
        {
            Logger.LogTrace("Home constructor");

            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            var _localizer = localizerFactory.Create(type);
            var _localizer2 = localizerFactory.Create("SharedResource", assemblyName);
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
