using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HwApp1410931031.Security
{
    public class JwtObject
    {
        public string Account { get; set; }
        public string Role { get; set; }

        public string Expire { get; set; }
    }
}