using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(PMSContext dbContext)
        {
            _userService = new UserService(dbContext);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUser()
        {
            //_userService.GetUser(User.Identity.Name);
            return Ok(_userService.GetUser(User.Identity.Name));
        }
    }
}
