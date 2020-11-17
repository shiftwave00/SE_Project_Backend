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
        private readonly IConfiguration _configuration;
        private readonly AuthorizeService _authorizationService;
        private readonly UserService _userService;
        private readonly JwtHelper _jwtHelper;
        public AuthorizeController(IConfiguration configuration, JwtHelper jwt)
        {
            _configuration = configuration;
            _authorizationService = new AuthorizeService(configuration);
            _userService = new UserService();
            _jwtHelper = jwt;
        }

        [HttpPost("github")]
        public async Task<IActionResult> AuthenticateGithub(RequestGithubLoginDto dto)
        {
            string accessToken = await _authorizationService.RequestGithubAccessToken(dto.Code);

            if (!string.IsNullOrEmpty(accessToken))
            {
                UserModel userModel = await _authorizationService.RequestGithubUserInfo(accessToken);

                if (!_userService.CheckUserExist(userModel.Account))
                {
                    _userService.CreateUser(userModel);
                }

                return Ok(_jwtHelper.GenerateToken(userModel.Account));
            }
            else
            {
                throw new Exception("error code");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckAuthentucate()
        {
            return Ok(User.Identity.Name);
        }
    }
}
