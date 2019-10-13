using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Web.Models.v1.Classifys
{
    public class CreateUpdateClassifyDto
    {
        [Required(ErrorMessage = "编码为必填项")]
        public string ClassifyCode { get; set; }
        public int SortCode { get; set; }
        [Required(ErrorMessage = "分类专栏为必填项")]
        public string ClassifyName { get; set; }
    }
}
