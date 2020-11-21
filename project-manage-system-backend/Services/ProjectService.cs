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
    public class ProjectService
    {
        public void Create(ProjectDto projectDto)
        {
            using (var dbContext = new PMSContext())
            {
                var user = dbContext.Users.Find(projectDto.UserId);
                if (user != null)
                {
                    var project = new Models.Project
                    {
                        Name = projectDto.ProjectName,
                        Owner = user
                    };

                    user.Projects.Add(project);
                }
                else
                {
                    throw new Exception("user fail，can not find this user");
                }


                if(dbContext.SaveChanges() == 0)
                {
                    throw new Exception("create project fail");
                }
            }
        }

        public List<ProjectResultDto> GetProjectByUserAccount(string account)
        {
            using (var dbContext = new PMSContext())
            {
                var user = dbContext.Users.Include(u => u.Projects).FirstOrDefault(u => u.Account.Equals(account));
                var query = (from p in user.Projects
                            select new ProjectResultDto { Id = p.ID, Name = p.Name }).ToList();
                return query;
            }
        }
    }
}
