using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class RepoIssuesDto
    { 
        public string averageDealwithIssueTime { get; set; }

        public List<ResponseGithubRepoIssuesDto> openIssues { get; set; }

        public List<ResponseGithubRepoIssuesDto> closeIssues { get; set; }
    }
}
