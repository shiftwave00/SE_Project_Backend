using project_manage_system_backend.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class WeekTotalData
    {
        public string Week { get; set; }

        public int Total { get; set; }
    }

    public class DayOfWeek
    {
        public string Day { get; set; }

        public int Commit { get; set; }
    }

    public class DayOfWeekData
    {
        public string Week { get; set; }

        public List<DayOfWeek> DetailDatas { get; set; }
    }

    public class CommitInfoDto
    {
        public List<WeekTotalData> WeekTotalData { get; set; }

        public List<DayOfWeekData> DayOfWeekData { get; set; }
    }
}
