using AspNetCoreComponentLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore
{
    public class SanitizeIM
    {
        public string Html { get; set; }
    }

    public class SanitizeVM //:BaseVM
    {
        public SanitizeIM Input { get; set; }
        public string SanitizedHtml { get; set; }

        public SanitizeVM(SanitizeIM input) : base()
        {
            Input = input;
        }

    }


}
