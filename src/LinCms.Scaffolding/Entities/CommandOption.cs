namespace LinCms.Scaffolding.Entities
{
    public class CommandOption
    {
        public string Directory { get; set; } = null!;
        public string Entity { get; set; } = null!;
        /// <summary>
        /// 是否拆分DTO
        /// </summary>
        public bool SeparateDto { get; set; }
        public bool CustomRepository { get; set; }
        public bool NoOverwrite { get; set; }
        public bool SkipEntityConstructors { get; set; }
    }
}
