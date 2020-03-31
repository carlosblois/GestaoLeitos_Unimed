using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Usuario.API
{

        public class User
        {
            public string UserID { get; set; }
            public string AccessKey { get; set; }
        }

        public class TokenConfigurations
        {
            public string Audience { get; set; }
            public string Issuer { get; set; }
            public int Seconds { get; set; }
        }
    
}
