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
        /// <summary>
        /// Пример переопределния культуры по умолчанию
        /// </summary>
        public override string DefaultCulture { get { return "ru"; } }

        public MenusController(IStorage storage, ILoggerFactory loggerFactory, IStringLocalizerFactory localizerFactory, IStringLocalizer localizer) 
            : base(storage, loggerFactory, localizerFactory, localizer)
        {
        }
    }
}