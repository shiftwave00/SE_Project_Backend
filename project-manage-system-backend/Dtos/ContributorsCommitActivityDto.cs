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
    /// <summary>
    /// w - Start of the week, given as a Unix timestamp.
    /// a - Number of additions
    /// d - Number of deletions
    /// c - Number of commits
    /// w_s - convert w to string date of week
    /// </summary>
    public class Week
    {
        public int w { get; set; }
        public int a { get; set; }
        public int d { get; set; }
        public int c { get; set; }
        public string w_s { get; set; }
    }

    public class ContributorsCommitActivityDto
    {
        public Author author { get; set; }
        public List<Week> weeks { get; set; }
        public int total { get; set; }
    }
}
