using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using project_manage_system_backend.Models;

namespace project_manage_system_backend.Dtos
{
    public class ProjectDto
    {
        public string ProjectName { set; get; }
        public string UserId { set; get; }
    }
}
