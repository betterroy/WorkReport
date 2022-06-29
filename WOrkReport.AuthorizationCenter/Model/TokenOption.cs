using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkReport.AuthorizationCenter.Model
{
    public class TokenOption
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
