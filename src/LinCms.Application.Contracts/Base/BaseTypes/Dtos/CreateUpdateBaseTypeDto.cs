using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Base.BaseTypes.Dtos
{
    public class CreateUpdateBaseTypeDto
    {
        [Required(ErrorMessage = "类别编码为必填项")]
        public string TypeCode { get; set; }
        [Required(ErrorMessage = "类别名称为必填项")]
        public string FullName { get; set; }
        public int? SortCode { get; set; }
    }
}
