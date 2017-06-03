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
    [AdminControllerSettings(LocalizerPrefix = "sites", Priority = 10)]
    public class SitesController : ControllerEditable<long, Sites, ISiteRepository>
    {
        public SitesController(IControllerSettings settings) : base(settings)
        {
        }
    }

    [AdminControllerSettings(LocalizerPrefix = "menus", Priority = 20)]
    public class MenusController : ControllerEditable<long, Menus, IMenuRepository>
    {

        public MenusController(IControllerSettings settings) : base(settings)
        {
        }
    }

    [AdminControllerSettings(LocalizerPrefix = "languages", Priority = 30)]
    public class LanguagesController : ControllerEditable<long, Languages, ILanguageRepository>
    {
        public LanguagesController(IControllerSettings settings) : base(settings)
        {
        }
    }

}
