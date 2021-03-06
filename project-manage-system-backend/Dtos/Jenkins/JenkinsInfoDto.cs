using System.Collections.Generic;



namespace project_manage_system_backend.Dtos.Jenkins
{
    public class JenkinsInfoDto
    {
        public string description { get; set; }
        public string name { get; set; }
        public List<HealthReport> healthReport { get; set; }

    }

    public class HealthReport
    {
        public string description { get; set; }
        public double score { get; set; }

    }
}
