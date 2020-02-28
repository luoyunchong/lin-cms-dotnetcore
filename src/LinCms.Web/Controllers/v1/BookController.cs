using AutoMapper;
using LinCms.Application.Contracts.v1.Books;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/book")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly AuditBaseRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        public BookController(AuditBaseRepository<Book> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除图书", "图书")]
        public UnifyResponseDto DeleteBook(int id)
        {
            _bookRepository.Delete(new Book { Id = id });
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        public PagedResultDto<Book> Get([FromQuery] PageDto pageDto)
        {
            return _bookRepository.Select.OrderByDescending(r => r.Id)
                .ToPagerList(pageDto, out long count)
                .ToPagedResultDto(count);
        }

        [HttpGet("{id}")]
        public BookDto Get(int id)
        {
            Book book = _bookRepository.Select.Where(a => a.Id == id).ToOne();
            return _mapper.Map<BookDto>(book);
        }

        [HttpPost]
        public UnifyResponseDto Post([FromBody] CreateUpdateBookDto createBook)
        {
            bool exist = _bookRepository.Select.Any(r => r.Title == createBook.Title);
            if (exist)
            {
                throw new LinCmsException("图书已存在");
            }

            Book book = _mapper.Map<Book>(createBook);
            _bookRepository.Insert(book);
            return UnifyResponseDto.Success("新建图书成功");
        }

        [HttpPut("{id}")]
        public UnifyResponseDto Put(int id, [FromBody] CreateUpdateBookDto updateBook)
        {
            Book book = _bookRepository.Select.Where(r => r.Id == id).ToOne();
            if (book==null)
            {
                throw new LinCmsException("没有找到相关书籍");
            }

            bool exist = _bookRepository.Select.Any(r => r.Title == updateBook.Title && r.Id != id);
            if (exist)
            {
                throw new LinCmsException("图书已存在");
            }

            //book.Image = updateBook.Image;
            //book.Title = updateBook.Title;
            //book.Author = updateBook.Author;
            //book.Summary = updateBook.Summary;
            //book.Summary = updateBook.Summary;

            //使用AutoMapper方法简化类与类之间的转换过程
            _mapper.Map(updateBook,book);

            _bookRepository.Update(book);

            return UnifyResponseDto.Success("更新图书成功");
        }
    }
}