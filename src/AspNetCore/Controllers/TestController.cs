using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using AspNetCoreComponentLibrary;
using System.Text.Encodings.Web;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace AspNetCore.Controllers
{
    public class TestController : Controller
    {
        public IStorage Storage;
        public readonly ILoggerFactory LoggerFactory;
        public readonly ILogger Logger;
        public readonly ILogger LoggerMEF;
        public readonly HtmlEncoder Encoder;
        public readonly IRnd Rnd;

        public ISiteRepository Sites { get; set; }
        public IUserRepository Users { get; set; }

        //public TestController(IStorage storage, ILoggerFactory loggerFactory)
        public TestController(IControllerSettings settings , HtmlEncoder enc, IRnd rnd)
        {
            LoggerFactory = settings.LoggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            LoggerMEF = LoggerFactory.CreateLogger(Utils.MEFNameSpace);
            Storage = settings.Storage;
            Encoder = enc;
            this.Rnd = rnd;
        }

        [HttpPost]
        public IActionResult Sanitize(SanitizeIM im)
        {
            Logger.LogTrace("Sanitize Post source = {0}", im.Html);
            var vm = new SanitizeVM(im);

            vm.SanitizedHtml = im.Html?.SanitizeHtml();// Encoder.Encode(im.Html);
            Logger.LogTrace("Sanitize Post SanitizedHtml = {0}", vm.SanitizedHtml);
            vm.Text = im.Html?.StripHtml();
            Logger.LogTrace("Sanitize Post Text = {0}", vm.Text);
            return View("Sanitize", vm);
        }

        public IActionResult Sanitize()
        {
            Logger.LogTrace("Sanitize Get");
            return View(new SanitizeVM( new SanitizeIM() { Html = "<b>Hello world!</b><br />\n привет мир<br />\n<script>console.log('alert')</script>" }));
        }

        public async Task<IActionResult> CheckLock()
        {
            //Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Storage.ConnectToSiteDB(1);
            var menus = Storage.GetRepository<IMenuRepository>(EnumDB.Content);
            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var tmp = menus.GetUnblocked(1);
                }));
            }
            await Task.WhenAll(tasks);
            return Utils.ContentResult("ok");
        }

        public IActionResult Profile(int v=0, int count=1000)
        {
            return Utils.ContentResult(_Profile(count, v));
        }

        public IActionResult GenMenu(int count = 3)
        {
            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            var site = Sites[1];
            if(site==null) return Utils.ContentResult("Site not founded!");
            Storage.ConnectToSiteDB(site.Id);

            var Menus = Storage.GetRepository<IMenuRepository>(EnumDB.Content);
            for (var i = 0; i < count; i++)
            {
                var name = "Menu " + i + " " + Rnd.RandomString(5);
                var menu = new Menus() { SiteId = site.Id, Name = name, MenuName = name, Priority = i, Content = Rnd.RandomString(1005) };
                Menus.Save(menu);
                Storage.Save(EnumDB.Content);
                Menus.AfterSave(menu, true);
                var parent = menu.Id;
                for (var j = 0; j < Rnd.Next(0, count); j++)
                {
                    name = "Menu " + i + "_" + j + " " + Rnd.RandomString(5);
                    menu = new Menus() { SiteId = site.Id, Name = name, MenuName = name, Priority = j, Content = Rnd.RandomString(1005), ParentId = parent };
                    Menus.Save(menu);
                    Storage.Save(EnumDB.Content);
                    Menus.AfterSave(menu, true);
                    var parent1 = menu.Id;
                    for (var k = 0; k < Rnd.Next(0, count); k++)
                    {
                        name = "Menu " + i + "_" + j + "_" + k + " " + Rnd.RandomString(5);
                        menu = new Menus() { SiteId = site.Id, Name = name, MenuName = name, Priority = k, Content = Rnd.RandomString(1005), ParentId = parent1 };
                        Menus.Save(menu);
                        Storage.Save(EnumDB.Content);
                        Menus.AfterSave(menu, true);
                    }
                }
            }
            
            return Utils.ContentResult("Ok");
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

                Storage.Save(EnumDB.UserSites);

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

                Storage.Save(EnumDB.UserSites);

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

                Storage.Save(EnumDB.UserSites);

                Users.AfterSave(user, false);
                Sites.AfterSave(site, false);
            }

            return Utils.ContentResult("ErrorTest Ok");
        }

        [NonAction]
        string _Profile(int iterations, int v)
        {
            //Run at highest priority to minimize fluctuations caused by other processes/threads
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            //Thread.CurrentThread.Priority = ThreadPriorityLevel.Highest;

            var coll = GenColl(100000);
            var watch = new Stopwatch();
            var res = string.Empty;

            if (v == 1)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                int c = -1;
                var query = coll.Values.AsParallel().Where(i => i.IsBlocked);
                watch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    //c = FilterStandart(coll);
                    c = FilterCommon(query);
                }
                watch.Stop();
                res += string.Format("1. Time Elapsed {0} ms {1}<br />\n", watch.Elapsed.TotalMilliseconds, c);
            }/**/

            if (v == 2)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var props = typeof(Languages).GetProperties();
                var p = props.Where(i => i.Name == "IsBlocked").FirstOrDefault();
                int cc = -1;
                if (p != null)
                {
                    var qquery = coll.Values.Where(i => (bool)p.GetValue(i, null));
                    watch.Start();
                    for (int i = 0; i < iterations; i++)
                    {
                        cc = FilterCommon(qquery);
                    }
                    watch.Stop();
                }
                res += string.Format("2. Time Elapsed {0} ms {1}<br />\n", watch.Elapsed.TotalMilliseconds, cc);
            }/**/

            if (v == 3)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                //var props = typeof(Languages).GetProperties();
                //var p = props.Where(i => i.Name == "IsBlocked").FirstOrDefault();
                int ccc = -1;
                //if (p != null)

                var qqquery = coll.Values.AsQueryable().Where(ExpressionHelper.ComparePropertyWithConst<Languages, bool>("IsBlocked", true));
                watch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    ccc = FilterCommon(qqquery);
                }
                watch.Stop();

                res += string.Format("3. Time Elapsed {0} ms {1}<br />\n", watch.Elapsed.TotalMilliseconds, ccc);
            }/**/

            return res;
        }

        [NonAction]
        int FilterCommon(IEnumerable<Languages> coll)
        {
            return coll.Count();
        }

        [NonAction]
        int FilterStandart(Dictionary<long, Languages> coll)
        {
            return coll.Values.Where(i => i.IsBlocked).Count();
        }

        [NonAction]
        int FilterOur(Dictionary<long, Languages> coll, PropertyInfo p)
        {
            return coll.Values.Where(i => (bool)p.GetValue(i, null)).Count();
            //return coll.Values.Where(i => (bool)(i.GetType().GetProperty("IsBlocked").GetValue(i, null))).Count();
        }

        [NonAction]
        Dictionary<long, Languages> GenColl(int items)
        {
            var coll = new Dictionary<long, Languages>();
            var rnd = new Rnd();
            for (var i = 0; i < items; i++)
            {
                coll[i] = new Languages() { Id = i, IsBlocked = rnd.Next(0, 99) >= 50, Lang = rnd.RandomString(5) };
            }
            return coll;
        }
    }
}