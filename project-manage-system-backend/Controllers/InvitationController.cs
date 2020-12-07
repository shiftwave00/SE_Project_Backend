using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System;

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
        [HttpGet("checkowner/{id}")]
        public IActionResult IsOwner(int id)
        {
            if (_userService.IsProjectOwner(_userService.GetUserModel(User.Identity.Name), id))
            {
                return Ok(new ResponseDto
                {
                    success = true,
                    message = "專案擁有者，可以邀請人"
                });
            }

            return Ok(new ResponseDto
            {
                success = false,
                message = "非專案的擁有者，無法邀請其他人加入專案"
            });
        }

        [Authorize]
        [HttpPost("users")]
        public IActionResult Invite(InvitationDto invitationDto)
        {
            if (_userService.IsProjectOwner(_userService.GetUserModel(User.Identity.Name), invitationDto.ProjectId))
            {
                return Ok(_userService.GetAllUser(User.Identity.Name));
            }

            return Ok(new ResponseDto
            {
                success = false,
                message = "非專案的擁有者，無法邀請其他人加入專案"
            });
        }

        [Authorize]
        [HttpPost("sendinvitation")]
        public IActionResult SendInvitation(InvitationDto invitationDto)
        {
            if (_userService.CheckUserExist(invitationDto.ApplicantId))
            {
                User inviter = _userService.GetUserModel(User.Identity.Name);
                User applicant = _userService.GetUserModel(invitationDto.ApplicantId);
                Project project = _repoService.GetProjectByProjectId(invitationDto.ProjectId);

                if (_userService.IsProjectOwner(inviter, invitationDto.ProjectId) && !_invitationService.IsUserInProject(applicant, project))
                {
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
                                success = false,
                                message = "已送出邀請，請勿重複邀請"
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        return NotFound(e);
                    }
                }
                else
                {
                    return Ok(new ResponseDto
                    {
                        success = false,
                        message = "使用者: " + applicant.Name + " 已在專案中"
                    });
                }
                
            }

            return Ok(new ResponseDto
            {
                success = false,
                message = "找不到該使用者，請輸入正確的使用者名稱"
            });
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetInvitations()
        {
            User user = _userService.GetUserModel(User.Identity.Name);
            var result = _invitationService.GetInvitations(user);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("reply")]
        public IActionResult ReplyToInvitation(ReplyToInvitationDto replyToInvitationDto)
        {
            Invitation invitation = _invitationService.GetInvitation(replyToInvitationDto.InvitationId);

            if (replyToInvitationDto.IsAgreed)
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
