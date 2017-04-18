using AspNetCoreComponentLibrary;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace AspNetCore.Test
{
    public class CheckBackUrlTests
    {
        public ISiteRepository Sites { get; set; }

        public CheckBackUrlTests() {
            Sites = new SiteRepositoryTest();
        }

        [Theory]
        [InlineData("localhost")]
        [InlineData("2garin.com")]
        [InlineData("www.2garin.com")]
        public void TestValidHosts(string host)
        {
            //var controller = new Controller2Garin(null, null);
            //controller.ControllerContext = new ControllerContext();
            //controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.True(Utils.CheckBackUrl(Sites, host));
        }
    }
}
