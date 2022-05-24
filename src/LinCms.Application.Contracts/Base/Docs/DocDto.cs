using LinCms.Entities;
namespace LinCms.Base.Docs
{
    public class DocDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

    }
}
