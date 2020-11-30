using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class ProjectService : BaseService
    {
        public ProjectService(PMSContext dbContext) : base(dbContext) { }

        public void Create(ProjectDto projectDto)
        {
            var user = _dbContext.Users.Find(projectDto.UserId);
            if (user != null)
            {
                var project = new Models.UserProject
                {
                    Project = new Models.Project { Name = projectDto.ProjectName, Owner = user },
                };

                user.Projects.Add(project);
            }
            else
            {
                throw new Exception("user fail，can not find this user");
            }


            if(_dbContext.SaveChanges() == 0)
            {
                throw new Exception("create project fail");
            }
        }

        public List<ProjectResultDto> GetProjectByUserAccount(string account)
        {
            var user = _dbContext.Users.Include(u => u.Projects).FirstOrDefault(u => u.Account.Equals(account));
            var query = (from up in user.Projects
                        select new ProjectResultDto { Id = up.Project.ID, Name = up.Project.Name }).ToList();
            return query;
        }
    }
}
