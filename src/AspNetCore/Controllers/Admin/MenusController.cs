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
    [AdminControllerSettings(LocalizerPrefix = "menus", Priority = 20)]
    public class MenusController : ControllerEditable<long, Menus, IMenuRepository>
    {

        public MenusController(IControllerSettings settings) : base(settings)
        {
        }
    }
}