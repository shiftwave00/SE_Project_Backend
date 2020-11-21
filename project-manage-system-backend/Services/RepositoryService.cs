using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class RepositoryService
    {
        public List<RepositoryModel> GetRepositoryByProjectId(int id)
        {
            using (var dbContent = new PMSContext())
            {
                var project = dbContent.Projects.Include(p => p.Repositories).FirstOrDefault(p => p.ID.Equals(id));
                return project.Repositories;
            }
        }
    }
}
