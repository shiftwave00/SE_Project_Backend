using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Services;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RepositoryController : ControllerBase
    {
        private readonly RepositoryService _repositoryService;

        public RepositoryController()
        {
            _repositoryService = new RepositoryService();
        }

        [HttpGet("{id}")]
        public IActionResult GetRepositoryByProjectId(int id)
        {
            var result = _repositoryService.GetRepositoryByProjectId(id);
            return Ok(result);
        }
    }
}