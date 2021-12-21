using System.Collections.Generic;

namespace project_manage_system_backend.Dtos
{
    public class BugDataDto
    {
        public int total { get; set; }
        public List<Issues> issues { get; set; }
        //因為格式一樣，先試試看能不能沿用codeSmell的
    }
}
