using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos.Jenkins
{
    public class JenkinsJobInfoDto
    {
        public List<Builds> builds { get; set; }

    }

    public class Builds
    {
        public string result { get; set; }

        public string displayName { get; set; }

        public int number { get; set; }

        public int duration { get; set; }

        public double timestamp { get; set; }
    }
}
