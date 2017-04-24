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
            Logger.LogTrace("Home constructor");
        }

        public IActionResult Index(PageIM im)
        {
            Logger.LogTrace("Home index");
            return im.ToActionResult(this);
        }

    }
}
