using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class AuthorizeDto
    {
        public string Token { get; set; }

        /// <summary>
        /// 第三方token
        /// </summary>
        public string OauthToken { get; set; }
    }
}
