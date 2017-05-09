using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using Microsoft.Extensions.Logging;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Localization;

namespace AspNetCore
{
    [AdminControllerSettings(MenuName = "menus.name", Priority = 10)]
    public class MenusController : ControllerEditable<long, Menus, IMenuRepository>
    {
        public MenusController(IStorage storage, ILoggerFactory loggerFactory, IStringLocalizerFactory localizerFactory) : base(storage, loggerFactory, localizerFactory)
        {
        }
    }
}