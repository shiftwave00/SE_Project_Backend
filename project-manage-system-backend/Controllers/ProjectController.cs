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
        private PMSContext _dbContext;

        public ProjectController(PMSContext dbContext)
        {
            _dbContext = dbContext;
            _projectService = new ProjectService(_dbContext);
            _userService = new UserService(_dbContext);
        }

        [HttpPost]
        public IActionResult AddProject(ProjectDto projectDto)
        {
            try
            {
                _projectService.Create(projectDto);
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

        [HttpPost("edit")]
        public IActionResult EditProjectName(ProjectDto projectDto)
        {
            try
            {
                if (CheckUserIsProjectOwner(projectDto.UserId, projectDto.ProjectId))
                {
                    _projectService.EditProjectName(projectDto);
                    return Ok(new ResponseDto
                    {
                        success = true,
                        message = "更改成功",
                    });
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                return Ok(new ResponseDto
                {
                    success = false,
                    message = ex.Message,
                });
            }

        }

        [Authorize]
        [HttpPost("delete")]
        public IActionResult DeleteProject(DeleteProjectDto projectDto)
        {
            try
            {
                if (CheckUserIsProjectOwner(projectDto.UserId, projectDto.ProjectId))
                {
                    _projectService.DeleteProject(projectDto.ProjectId);

                    return Ok(new ResponseDto
                    {
                        success = true,
                        message = "刪除成功",
                    });
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                return NotFound(new ResponseDto
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetProject()
        {
            var result = _projectService.GetProjectByUserAccount(User.Identity.Name);
            return Ok(result);
        }


        [HttpPost("get")]
        public IActionResult GetProject(ProjectDto projectDto)
        {
            var result = _projectService.GetProjectByProjectId(projectDto);
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