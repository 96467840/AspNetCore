using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Controllers
{
    // этот контролер получает управление тока тогда когда БД еще нет и потому его нельзя неаследовать от Controller2Garin
    public class SetupController : Controller
    {
        public IStorage Storage;
        public readonly ILoggerFactory LoggerFactory;
        public readonly ILogger Logger;

        public SetupController(IStorage storage, ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            Storage = storage;
        }

        public IActionResult Index(SetupIM input)
        {
            return input.ToActionResult(this, Storage, LoggerFactory);
        }
    }
}