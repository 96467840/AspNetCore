using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AspNetCore
{
    [AdminControllerSettings(MenuName = "languages.name", Priority = 30)]
    public class LanguagesController : ControllerEditable<long, Languages, ILanguageRepository>
    {
        public LanguagesController(IStorage storage, ILoggerFactory loggerFactory, IStringLocalizerFactory localizerFactory, IStringLocalizer localizer) 
            : base(storage, loggerFactory, localizerFactory, localizer)
        {
        }
    }
}