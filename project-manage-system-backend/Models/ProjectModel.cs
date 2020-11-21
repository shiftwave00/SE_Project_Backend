using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using project_manage_system_backend.Models;

namespace project_manage_system_backend.Models
{
    public class ProjectModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public UserModel Owner { get; set; }
        
        public List<RepoModel> Repositories { get; } = new List<RepoModel>();
    }
}
