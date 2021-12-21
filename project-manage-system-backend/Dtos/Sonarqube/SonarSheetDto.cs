using System.Collections.Generic;


namespace project_manage_system_backend.Dtos {
    public class SonarSheetDto
    {
        public Component baseComponent { get; set; }
        public List<Component> components { get; set; }
    }

    public class Component
    {
        public string key { get; set; }
        public string qualifier { get; set; }
        public List<Measures> measures { get; set; }
    }

    public class Measures
    {
        public string metric { get; set; }
        public string value { get; set; }
    }
}
