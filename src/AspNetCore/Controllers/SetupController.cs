using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary;

namespace AspNetCore.Controllers
{
    // этот контролер получает управление тока тогда когда БД еще нет и потому его нельзя неаследовать от Controller2Garin
    public class SetupController : Controller
    {
        public IActionResult Index(SetupIM input)
        {
            return input.ToActionResult(this);
        }
    }
}