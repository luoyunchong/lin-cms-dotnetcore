using AutoMapper;
using LinCms.Web.Models.v1.Books;
using LinCms.Zero.Domain;

namespace LinCms.Web.AutoMapper.v1
{
    public class BookProfile:Profile
    {
        public BookProfile()
        {
            CreateMap<CreateUpdateBookDto, Book>();
            CreateMap<Book, BookDto>();
        }
    }
}
