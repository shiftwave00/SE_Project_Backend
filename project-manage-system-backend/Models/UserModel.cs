using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Models
{
    public class UserModel
    {
        [Key]
        public string Account { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public string Authority { get; set; }
    }
}
