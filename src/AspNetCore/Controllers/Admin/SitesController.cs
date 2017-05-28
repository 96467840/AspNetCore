using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;
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
}