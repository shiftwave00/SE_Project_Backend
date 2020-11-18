using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectController()
        {
            _projectService = new ProjectService();
        }

        [HttpPost]
        public IActionResult AddProject(ProjectDto projectDto)
        {
            _projectService.Create(projectDto);
            return Ok();
        }

        [HttpGet("{encryptUserId}")]
        public IActionResult GetProject(string encryptUserId)
        {
            var result = _projectService.GetProjectByUserAccount(encryptUserId);
            return Ok(result);
        }
    }
}