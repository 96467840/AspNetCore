using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using Microsoft.Extensions.Logging;
using AspNetCoreComponentLibrary.Abstractions;

namespace AspNetCore.Controllers
{
    public class HomeController : Controller2Garin
    {

        public HomeController(IStorage storage, ILoggerFactory loggerFactory) : base(storage, loggerFactory)
        {
            Logger.LogInformation("Home constructor", new object[0]);
        }

        public IActionResult Index(PageIM im)
        {
            Logger.LogInformation("Home index", new object[0]);
            return im.ToActionResult(this);
        }

    }
}
