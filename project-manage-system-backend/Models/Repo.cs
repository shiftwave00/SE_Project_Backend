using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using project_manage_system_backend.Models;

namespace project_manage_system_backend.Models
{
    public class Repo
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
        /// <summary>
        /// 實際 repository (第三方) 的擁有者名稱
        /// </summary>
        public string Owner { get; set; }

        //public Project SourceProject { get; set; }
    }
}
