using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;
using Microsoft.Extensions.Logging;
using AspNetCoreComponentLibrary.Abstractions;

namespace AspNetCore
{
    public class MenusController : ControllerEditable<long, Menus, IMenuRepository>
    {
        public MenusController(IStorage storage, ILoggerFactory loggerFactory) : base(storage, loggerFactory)
        {
        }
    }
}