﻿using AspNetCoreComponentLibrary;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace AspNetCore.Test
{
    public class CultureTests
    {
        [Theory]
        [InlineData("ru")]
        [InlineData("ru-RU")]
        public void TestValidCulture(string culture)
        {
            Assert.True(culture.TestCulture());
        }

        [Theory]
        [InlineData("")]
        [InlineData("r")]
        [InlineData("--")]
        [InlineData("1u-ru")]
        [InlineData(".u-ru")]
        [InlineData("ru-!U")]
        [InlineData("1u")]
        [InlineData("ruRU")]
        [InlineData("ru--U")]
        [InlineData("ru---")]
        [InlineData("ru-qwe")]
        [InlineData("-----")]
        [InlineData("----")]
        public void TestNotValidCulture(string culture)
        {
            Assert.False(culture.TestCulture());
        }
    }
}