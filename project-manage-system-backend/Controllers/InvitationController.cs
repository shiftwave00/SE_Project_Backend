using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
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
    public class InvitationController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly InvitationService _invitationService;
        private readonly RepoService _repoService;

        public InvitationController(PMSContext dbContext)
        {
            _userService = new UserService(dbContext);
            _invitationService = new InvitationService(dbContext);
            _repoService = new RepoService(dbContext);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Invite(string Applicant, int projectId)
        {
            if (_userService.CheckUserExist(Applicant))
            {
                User inviter = _userService.GetUserModel(User.Identity.Name);
                User applicant = _userService.GetUserModel(Applicant);
                Project project = _repoService.GetProjectByProjectId(projectId);

                try
                {
                    _invitationService.CreateInvitation(inviter, applicant, project);

                    // todo
                    // sendMsg to applicant

                    return Ok(new ResponseDto
                    {
                        success = true,
                        message = "送出邀請"
                    });
                }
                catch (Exception e)
                {
                    return NotFound(e);
                }
            }

            return Ok(new ResponseDto
            {
                success = false,
                message = "找不到使用者: " + Applicant
            });
        }
    }
}
