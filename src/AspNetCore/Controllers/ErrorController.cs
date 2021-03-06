using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Controllers
{
    // этот контролер получает управление когда произошла ошибка (возможно что сайт не найден)
    public class ErrorController : Controller
    {
        public IStorage Storage;
        public readonly ILoggerFactory LoggerFactory;
        public readonly ILogger Logger;

        public ISiteRepository Sites { get; set; }
        public IUserRepository Users { get; set; }

        public ErrorController(IStorage storage, ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            Storage = storage;

            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error404(Exception e)
        {
            return View(e);
        }

        public IActionResult Error500(Exception e)
        {
            return View(e);
        }
    }
}