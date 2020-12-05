using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class CodebaseDto
    {
        public string Date { get; set; }
        public int NumberOfRowsAdded { get; set; }
        public int NumberOfRowsDeleted { get; set; }

        public int NumberOfRows { get; set; }
    }
}
