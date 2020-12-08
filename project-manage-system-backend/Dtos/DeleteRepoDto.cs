using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class DeleteRepoDto
    {
        public int projectId { get; set; }

        public int repoId { get; set; }
    }
}
