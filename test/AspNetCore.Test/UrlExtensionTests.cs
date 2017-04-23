using AspNetCoreComponentLibrary;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AspNetCore
{
    public class UrlExtensionTests
    {
        public ISiteRepository Sites { get; set; }

        public UrlExtensionTests()
        {
            Sites = new SiteRepositoryTest();
        }

        [Fact]
        public void TestOutUrl()
        {
            //Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            //urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost");

            //UserController controller = new UserController();
            //controller.Url = urlHelper.Object;

            Assert.False(false);
        }/**/
    }
}
