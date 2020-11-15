using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class RequestOauthDto
    {
        public string Client_id { get; set; }

        public string Client_secret { get; set; }

        public string Code { get; set; }
    }
}
