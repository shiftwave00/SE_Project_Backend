using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace project_manage_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OauthController : ControllerBase
    {
        [HttpPost("github")]
        public IActionResult AuthenticateGithubToken(string code)
        {
            return Ok();
        }
    }
}
