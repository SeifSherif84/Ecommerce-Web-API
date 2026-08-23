using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Shared
{
    public class JWTOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecurityKey { get; set; } = string.Empty;
        public double ExpiredDurationInMinute { get; set; }

    }
}
