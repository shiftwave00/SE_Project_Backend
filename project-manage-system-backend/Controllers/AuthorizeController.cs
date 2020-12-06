using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System.Linq;
using System.Threading.Tasks;

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
            return Ok(User.Claims.Where(c => c.Type.Equals("oauth")).FirstOrDefault().Value);
        }
    }
}
