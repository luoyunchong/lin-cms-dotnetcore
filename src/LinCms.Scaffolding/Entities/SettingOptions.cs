using Humanizer;

namespace LinCms.Scaffolding.Entities
{
    public class SettingOptions
    {
        public static string Name = "SettingOptions";
        public string BaseDirectory { get; set; }
        public string ProjectName { get; set; }
        public string Areas { get; set; }
        public string EntityFilePath { get; set; }
        public string TemplatePath { get; set; }
        public string OutputDirectory { get; set; }
        //区域小写
        public string AreasCamelize => Areas.Camelize();

    }
}
