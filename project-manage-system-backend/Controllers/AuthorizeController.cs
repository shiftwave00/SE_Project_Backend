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

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AuthorizeService _authorizationService;
        public AuthorizeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _authorizationService = new AuthorizeService(configuration);
        }

        [HttpPost("github")]
        public IActionResult AuthenticateGithub(RequestGithubLoginDto dto)
        {
            _authorizationService.RequestGithubAccessToken(dto.Code);

            return Ok();
        }
    }
}
