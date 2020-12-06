using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(_userService.GetUser(User.Identity.Name));
        }
    }
}
