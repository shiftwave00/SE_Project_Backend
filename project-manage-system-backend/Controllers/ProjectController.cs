using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly UserService _userService;
        private readonly PMSContext _dbContext;

        public ProjectController(PMSContext dbContext)
        {
            _dbContext = dbContext;
            _projectService = new ProjectService(_dbContext);
            _userService = new UserService(_dbContext);
        }

        [Authorize]
        [HttpPost("add")]
        public IActionResult AddProject(ProjectDto projectDto)
        {
            try
            {
                _projectService.CreateProject(projectDto, User.Identity.Name);
                return Ok(new ResponseDto
                {
                    success = true,
                    message = "新增成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseDto
                {
                    success = false,
                    message = "新增失敗" + ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("edit")]
        public IActionResult EditProjectName(ProjectDto projectDto)
        {
            try
            {
                CheckUserIsProjectOwner(User.Identity.Name, projectDto.ProjectId);
                _projectService.EditProjectName(projectDto);
                return Ok(new ResponseDto
                {
                    success = true,
                    message = "更改成功",
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseDto
                {
                    success = false,
                    message = ex.Message,
                });
            }

        }

        [Authorize]
        [HttpDelete("{projectId}/{userId}")]
        public IActionResult DeleteProject(int projectId, string userId)
        {
            try
            {
                CheckUserIsProjectOwner(userId, projectId);

                _projectService.DeleteProject(projectId);

                return Ok(new ResponseDto
                {
                    success = true,
                    message = "刪除成功",
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseDto
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        [Authorize]
        [HttpDelete("{projectId}/{userId}")]
        public IActionResult DeleteProjectMember(int projectId, string userId)
        {
            try
            {
                CheckUserIsProjectOwner(User.Identity.Name, projectId);
                if (_userService.CheckUserExist(userId))
                {
                    var user = _userService.GetUserModel(userId);
                    if (!_userService.IsProjectOwner(user, projectId))
                    {
                        _projectService.DeleteProjectMember(userId, projectId);

                        return Ok(new ResponseDto
                        {
                            success = true,
                            message = "刪除成功",
                        });
                    }
                }
                else
                {
                    return Ok(new ResponseDto
                    {
                        success = false,
                        message = "該使用者不存在",
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResponseDto
                {
                    success = false,
                    message = ex.Message,
                });
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetProject()
        {
            var result = _userService.IsAdmin(User.Identity.Name) ? _projectService.GetAllProject() : _projectService.GetProjectByUserAccount(User.Identity.Name);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{projectId}")]
        public IActionResult GetProject(int projectId)
        {
            var result = _projectService.GetProjectByProjectId(projectId, User.Identity.Name);
            return Ok(result);
        }

        private bool CheckUserIsProjectOwner(string userId, int projectId)
        {
            if (_userService.CheckUserExist(userId))
            {
                var user = _userService.GetUserModel(userId);
                if (_userService.IsProjectOwner(user, projectId))
                {
                    return true;
                }
                else
                {
                    throw new Exception("you are not the project owner");
                }
            }
            else
            {
                throw new Exception("you are not the system user");
            }
        }
    }
}