using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.v1.Books.Dtos
{
    public class CreateUpdateBookDto
    {
        [Required(ErrorMessage = "必须传入图书作者")]
        [StringLength(30,ErrorMessage = "图书作者应小于30字符")]
        public string Author { get; set; } 

        [Required(ErrorMessage = "必须传入图书插图")]
        [StringLength(50, ErrorMessage = "图书插图应小于50字符")]
        public string Image { get; set; } 

        [Required(ErrorMessage = "必须传入图书综述")]
        [StringLength(1000, ErrorMessage = "图书综述应小于1000字符")]
        public string Summary { get; set; } 

        [Required(ErrorMessage = "必须传入图书名")]
        [StringLength(50, ErrorMessage = "图书名应小于50字符")]
        public string Title { get; set; }
    }
}
