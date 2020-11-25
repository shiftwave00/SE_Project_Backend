using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{

    public class Author
    {
        public string login { get; set; }
        public string avatar_url { get; set; }
        public string html_url { get; set; }
    }

    public class Week
    {
        public int w { get; set; }
        public int a { get; set; }
        public int d { get; set; }
        public int c { get; set; }
    }

    public class AllContributorCommitActivityApiDto
    {
        public Author author { get; set; }
        public List<Week> weeks { get; set; }
        public int total { get; set; }
    }
}
