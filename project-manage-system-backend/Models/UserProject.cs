using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Models
{
    public class UserProject
    {
        public string Account { get; set; }
        public User User { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
