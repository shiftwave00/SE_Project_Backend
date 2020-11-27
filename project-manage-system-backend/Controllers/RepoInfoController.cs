using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RepoInfoController : ControllerBase
    {
        private readonly RepoInfoService _repoInfoService;
        public RepoInfoController(PMSContext dbContext)
        {
            _repoInfoService = new RepoInfoService(dbContext);
        }

        [HttpGet("commit/{repoId}")]
        public async Task<IActionResult> GetCommitInfo(int repoId)
        {
            return Ok(await _repoInfoService.RequestCommitInfo(repoId));
        }

        [Authorize]
        [HttpGet("contribute/{repoId}")]
        public async Task<IActionResult> GetContributorsActtivity(int repoId)
        {
            string oauth_token = User.Claims.Where(c => c.Type.Equals("oauth")).FirstOrDefault().Value;
            return Ok(await _repoInfoService.RequestContributorsActtivity(repoId, oauth_token));
        }
    }
}
