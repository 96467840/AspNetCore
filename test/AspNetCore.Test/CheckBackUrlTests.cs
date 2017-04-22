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
        [InlineData("http://localhost")]
        [InlineData("https://localhost/?ewrw=wer")]
        [InlineData("http://2garin.com")]
        [InlineData("//2garin.com")]
        [InlineData("http://www.2garin.com")]
        [InlineData("http://www.2garin.com/")]
        [InlineData("http://www.2garin.com/path/")]
        [InlineData("https://www.2garin.com/?param1=wsfhw")]
        [InlineData("https://www.2garin.com:8080/?param1=wsfhw")]
        [InlineData("//www.2garin.com")]
        [InlineData("relative/path")]
        [InlineData("/absolute/path")]
        public void TestValidHosts(string host)
        {
            //var controller = new Controller2Garin(null, null);
            //controller.ControllerContext = new ControllerContext();
            //controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.True(Utils.CheckBackUrl(Sites, host));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("http://ewrj")]
        [InlineData("http://localhost1")]
        [InlineData("http://2garin.ru")]
        [InlineData("http://2garin.ru/2garin.com")]
        [InlineData("//2garin.ru")]
        [InlineData("http://2garin.com.2garin.ru")]
        public void TestNotValidHosts(string host)
        {
            Assert.False(Utils.CheckBackUrl(Sites, host));
        }
    }
}
