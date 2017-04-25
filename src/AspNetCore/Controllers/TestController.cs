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

            var user = Users[1]; // ������ ����� � ��� ����� � �������
            var site = Sites[2]; // ������ ����

            if (site != null)
            {
                site.Name = "New name 2 " + DateTime.Now;

                Sites.Save(site);

                // ������� ����� ������������ user �� ���� site (���� �� ���)
                if (!user.UserSites.Any(i=>i.SiteId == site.Id))
                {
                    var us = new UserSites() { UserId = user.Id, SiteId = site.Id, Rights="rights "};
                    UserSites.Save(us);
                    //site.UserSites.Add(us);
                    
                    //user.UserSites.Add(us); // ��� ��� ���� ������ �� ����� ��� ��� �������� � ���������� ������ ����� ���� (������ � ������ ������)

                    //Users.Save(user); // ��� ������ �� ����� �� ���� ��� ���� �� ��������� ���������� ���� ����� (������ ����� ����� ������)

                    // ����������� ������� ����� �� ����
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

            var user = Users[1]; // ������ ����� � ��� ����� � �������
            var site = Sites[2]; // ������ ����

            if (site != null)
            {
                site.Name = "New name 2 " + DateTime.Now;

                Sites.Save(site);

                // ������� ����� ������������ user �� ���� site (���� �� ���)
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