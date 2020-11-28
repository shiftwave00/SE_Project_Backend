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
        public IActionResult Invite(InvitationDto invitationDto)
        {
            if (_userService.CheckUserExist(invitationDto.ApplicantId))
            {
                User inviter = _userService.GetUserModel(User.Identity.Name);
                User applicant = _userService.GetUserModel(invitationDto.ApplicantId);
                Project project = _repoService.GetProjectByProjectId(invitationDto.ProjectId);

                try
                {
                    var invitation = _invitationService.CreateInvitation(inviter, applicant, project);
                    if (!_invitationService.IsInvitationExist(invitation))
                    {
                        _invitationService.AddInvitation(invitation);
                        // todo
                        // sendMsg to applicant

                        return Ok(new ResponseDto
                        {
                            success = true,
                            message = "送出邀請"
                        });
                    }
                    else
                    {
                        return Ok(new ResponseDto
                        {
                            success = true,
                            message = "送出邀請"
                        });
                    }
                }
                catch (Exception e)
                {
                    return NotFound(e);
                }
            }

            return Ok(new ResponseDto
            {
                success = false,
                message = "找不到使用者: " + invitationDto.ApplicantId
            });
        }

        [Authorize]
        [HttpPost("Reply")]
        public IActionResult ReplyToInvitation(ReplyToInvitationDto replyToInvitationDto)
        {
            Invitation invitation = _invitationService.GetInvitation(replyToInvitationDto.InvitationId);

            if (invitation.IsAgreed)
            {
                _userService.AddProject(invitation);
            }

            try
            {
                _invitationService.DeleteInvitation(invitation);
                return Ok(new ResponseDto
                {
                    success = true,
                    message = "刪除成功"
                });
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }
    }
}
