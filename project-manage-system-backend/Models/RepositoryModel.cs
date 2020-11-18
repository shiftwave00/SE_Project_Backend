using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using project_manage_system_backend.Models;

namespace project_manage_system_backend.Models
{
    public class RepositoryModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Owner { get; set; }
    }
}
