using AutoMapper;
using LinCms.Application.Contracts.v1.Books;
using LinCms.Core.Entities;

namespace LinCms.Application.AutoMapper.Blog
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
