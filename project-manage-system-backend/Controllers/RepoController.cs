using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly RepoService _repoService;
        public RepoController(PMSContext dbContext)
        {
            _repoService = new RepoService(dbContext);
        }

        [HttpPost]
        public async Task<IActionResult> AddRepo(RequestAddRepoDto addRepoDto)
        {
            var response = await _repoService.CheckRepoExist(addRepoDto.url);
            if (response.IsSucess)
            {
                var project = _repoService.GetProjectByProjectId(addRepoDto.projectId);

                Repo model = new Repo()
                {
                    Name = response.name,
                    Owner = response.owner.login,
                    Url = response.html_url,
                    Project = project
                };
                try
                {
                    _repoService.CreateRepo(model);
                    return Ok(new ResponseDto
                    {
                        success = true,
                        message = "新增成功"
                    });
                }
                catch (Exception e)
                {
                    return Ok(new ResponseDto
                    {
                        success = false,
                        message = "新增失敗:" + e.Message
                    });

                }
            }
            else
            {
                return Ok(new ResponseDto
                {
                    success = false,
                    message = "新增失敗:" + response.message
                });

            }

        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetRepositoryByProjectId(int id)
        {
            var result = _repoService.GetRepositoryByProjectId(id);
            return Ok(result);
        }
    }
}
