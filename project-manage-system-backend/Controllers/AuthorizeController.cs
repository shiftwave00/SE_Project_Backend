using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Services;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using Microsoft.AspNetCore.Authorization;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly AuthorizeService _authorizationService;
        public AuthorizeController(IConfiguration configuration, PMSContext context, JwtHelper jwt)
        { 
            _authorizationService = new AuthorizeService(context, configuration, jwt);
        }

        [HttpPost("github")]
        public async Task<IActionResult> AuthenticateGithub(RequestGithubLoginDto dto)
        {
            return Ok(await _authorizationService.AuthenticateGithub(dto));
        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckAuthentucate()
        {
            return Ok(User.Identity.Name);
        }
    }
}
