﻿using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreComponentLibrary;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AspNetCore
{
    public class SiteRepositoryTest : ISiteRepository
    {
        private Dictionary<long, Sites> coll { get; set; }

        public SiteRepositoryTest()
        {
            coll = new Dictionary<long, Sites> {
                { 1, new Sites { Id = 1, Name = "Site1", Hosts = "  localhost, 2garin.com , *.2garin.com" } },
                { 2, new Sites { Id = 2, Name = "Site2", Hosts = " example.com" } },
                { 3, new Sites { Id = 3, Name = "Site3", Hosts = " example.com, *.example.com" } },

            };
        }

        public Sites this[long index]
        {
            get
            {
                //if (index == null) return default(Sites);
                if (!coll.ContainsKey(index)) return default(Sites);
                return coll[index];
            }
        }

        public void AfterSave(Sites item, bool isnew)
        {
            throw new NotImplementedException();
        }

        public void Block(long id)
        {
            throw new NotImplementedException();
        }

        public void Remove(Sites item)
        {
            throw new NotImplementedException();
        }

        public Sites Save(Sites item)
        {
            throw new NotImplementedException();
        }

        public void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory, ILocalizer2Garin localizer2Garin)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Sites> StartQuery(long siteid)
        {
            return coll.Values.AsQueryable();
        }

        public void UnBlock(long id)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromCache(long id)
        {
            throw new NotImplementedException();
        }

        public bool BeforeSave(Sites item)
        {
            throw new NotImplementedException();
        }

        public void AddToCache(long index)
        {
            throw new NotImplementedException();
        }

        public List<UserSites> GetUserRights(long siteid)
        {
            throw new NotImplementedException();
        }

        public void ClearCache(long? siteid)
        {
            throw new NotImplementedException();
        }

        public List<Sites> GetUnblocked(long siteid)
        {
            throw new NotImplementedException();
        }

        IQueryable<Sites> IRepository<long, Sites>.GetUnblocked(long siteid)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Sites> GetFiltered(long siteid, Form form)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Sites> GetForUser(long userid)
        {
            throw new NotImplementedException();
        }

        public void TranslateItem(long siteid, Languages lang, List<Languages> siteLanguages, Sites item)
        {
            throw new NotImplementedException();
        }

        public void TranslateList(long siteid, Languages lang, List<Languages> siteLanguages, List<Sites> items)
        {
            throw new NotImplementedException();
        }
    }
}
