using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using AspNetCoreComponentLibrary;
using System.Text.Encodings.Web;

namespace AspNetCore.Controllers
{
    public class TestController : Controller
    {
        public IStorage Storage;
        public readonly ILoggerFactory LoggerFactory;
        public readonly ILogger Logger;
        public readonly ILogger LoggerMEF;
        public readonly HtmlEncoder Encoder;

        public ISiteRepository Sites { get; set; }
        public IUserRepository Users { get; set; }

        //public TestController(IStorage storage, ILoggerFactory loggerFactory)
        public TestController(IControllerSettings settings , HtmlEncoder enc)
        {
            LoggerFactory = settings.LoggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            LoggerMEF = LoggerFactory.CreateLogger(Utils.MEFNameSpace);
            Storage = settings.Storage;
            Encoder = enc;
        }

        [HttpPost]
        public IActionResult Sanitize(SanitizeIM im)
        {
            Logger.LogTrace("Sanitize Post source = {0}", im.Html);
            var vm = new SanitizeVM(im);

            vm.SanitizedHtml = Encoder.Encode(im.Html);
            Logger.LogTrace("Sanitize Post SanitizedHtml = {0}", vm.SanitizedHtml);
            return View(vm);
        }

        public IActionResult Sanitize()
        {
            Logger.LogTrace("Sanitize Get");
            return View(new SanitizeVM( new SanitizeIM() { Html = "<b>Hello world!</b><br /> привет мир<br /><script>console.log('alert')</script>" }));
        }

        public IActionResult Index(string culture, string path)
        {
            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);
            var UserSites = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);

            var user = Users[1]; // грузим юзера и его связи с сайтами
            var site = Sites[2]; // грузим сайт

            if (site != null)
            {
                site.Name = "New name 2 " + DateTime.Now;

                Sites.Save(site);

                // добавим права пользователя user на сайт site (если их нет)
                var founded = user.UserSites.Any(i => i.SiteId == site.Id);
                if (!founded)
                {
                    var us = new UserSites() { UserId = user.Id, SiteId = site.Id, Rights="rights "};
                    UserSites.Save(us);
                }

                Storage.Save();

                Users.AfterSave(user, false);
                Sites.AfterSave(site, false);
            }


            return View();
        }

        public IActionResult DelRel(string culture, string path)
        {
            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);
            var UserSites = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);

            var user = Users[1]; // грузим юзера и его связи с сайтами
            var site = Sites[2]; // грузим сайт

            if (site != null)
            {
                site.Name = "New name 2 " + DateTime.Now;

                Sites.Save(site);

                // добавим права пользователя user на сайт site (если их нет)
                var founded = user.UserSites.Any(i => i.SiteId == site.Id && i.UserId == user.Id);
                if (founded)
                {
                    var us = new UserSites() { UserId = user.Id, SiteId = site.Id };
                    UserSites.Remove(us);
                }

                Storage.Save();

                Users.AfterSave(user, false);
                Sites.AfterSave(site, false);
            }


            return View();
        }

        public IActionResult ErrorTest(string culture, string path)
        {
            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);
            var UserSites = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);

            var user = Users[1]; // грузим юзера и его связи с сайтами
            var site = Sites[2]; // грузим сайт

            if (site != null)
            {
                site.Name = "New name 2 " + DateTime.Now;

                Sites.Save(site);

                // добавим права пользователя user на сайт site (с нарушением ограничения)
                {
                    var us = new UserSites() { UserId = user.Id, SiteId = site.Id, Rights = "rights " };
                    UserSites.Save(us);
                }

                Storage.Save();

                Users.AfterSave(user, false);
                Sites.AfterSave(site, false);
            }

            return Utils.ContentResult("ErrorTest Ok");
        }

    }
}