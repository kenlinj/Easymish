using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Easymish.WebUI.Models
{
    public class RegularExpressionReplacement
    {
        public string RegularExpression
        {
            get;
            set;
        }

        public string InputText
        {
            get;
            set;
        }

        public string ResultText
        {
            get;
            set;
        }
    }
}