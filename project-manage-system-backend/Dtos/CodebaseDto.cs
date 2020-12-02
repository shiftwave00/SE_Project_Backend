using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class CodebaseDto
    {
        public string date { get; set; }
        public int numberOfRowsAdded { get; set; }
        public int numberOfRowsDeleted { get; set; }

        public int numberOfRows { get; set; }
    }
}
