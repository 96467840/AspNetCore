using AspNetCoreComponentLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AspNetCore
{
    
    public class CheckDefaultTests
    {
        [Theory]
        [InlineData(null)]
        public void CheckDefaultForString_Valid(string input)
        {
            Assert.True(Utils.CheckDefault<string>(input));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)] // внезапно! возможно потому что при попытке передать в long null получим 0. получаем глюк тестов см. CheckDefault_ForLong_Set_null
        public void CheckDefaultForLong_Valid(long input)
        {
            Assert.True(Utils.CheckDefault<long>(input));
        }

        [Theory]
        [InlineData(null)]
        public void CheckDefault_ForLong_Set_null(long input)
        {
            // компилятор запрещает сие святотатство
            //input = null;
            Assert.True(input == 0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public void CheckDefaultForLong_NotValid(long input)
        {
            Assert.False(Utils.CheckDefault<long>(input));
        }


    }
}
