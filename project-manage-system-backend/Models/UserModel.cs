using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Models
{
    public class UserModel
    {
        /// <summary>
        /// 第三方的帳號 Id
        /// </summary>
        [Key]
        public string Account { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }
        /// <summary>
        /// 值有：
        ///     User
        ///     Admin
        /// </summary>
        public string Authority { get; set; }

        public List<ProjectModel> Projects { get; } = new List<ProjectModel>();
    }
}
