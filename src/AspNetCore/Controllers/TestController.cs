using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using AspNetCoreComponentLibrary;

namespace AspNetCore.Controllers
{
    public class TestController : Controller
    {
        public IStorage Storage;
        public readonly ILoggerFactory LoggerFactory;
        public readonly ILogger Logger;

        public ISiteRepository Sites { get; set; }
        public IUserRepository Users { get; set; }

        public TestController(IStorage storage, ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            Storage = storage;
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
                if (!user.UserSites.Any(i=>i.SiteId == site.Id))
                {
                    var us = new UserSites() { UserId = user.Id, SiteId = site.Id, Rights="rights "};
                    UserSites.Save(us);
                    //site.UserSites.Add(us);
                    
                    //user.UserSites.Add(us); // вот это тоже делать не стоит так как приводит к сохранению самого юзера тоже (причем в инсерт режиме)

                    //Users.Save(user); // это делать не будем по идее сам юзер не изменился изменилась тока связь (скорее всего через инсерт)

                    // обязательно удалить юзера из кеша
                    //Users.RemoveFromCache(user.Id);
                }

                Storage.Save();

                Users.AfterSave(user, false);
                Sites.AfterSave(site, false);

                Users.AddToCache(user.Id);
                Sites.AddToCache(site.Id);
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
                if (user.UserSites.Any(i => i.SiteId == site.Id && i.UserId==user.Id))
                {
                    var us = new UserSites() { UserId = user.Id, SiteId = site.Id, Rights = "rights " };
                    UserSites.Remove(us);
                }

                Storage.Save();

                Users.AfterSave(user, false);
                Sites.AfterSave(site, false);

                Users.AddToCache(user.Id);
                Sites.AddToCache(site.Id);
            }


            return View();
        }

    }
}