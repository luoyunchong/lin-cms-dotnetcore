using System.Collections.Generic;
using AutoMapper;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Models.v1.Books;
using LinCms.Web.Repositories;
using LinCms.Zero;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Repositories;
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
        public ResultDto DeleteBook(int id)
        {
            _bookRepository.Delete(new Book { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public List<Book> Get()
        {
            return _bookRepository.Select.OrderByDescending(r=>r.Id).ToList();
        }

        [HttpGet("{id}")]
        public BookDto Get(int id)
        {
            Book book = _bookRepository.Select.Where(a => a.Id == id).ToOne();
            return _mapper.Map<BookDto>(book);
        }

        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateBookDto createBook)
        {
            bool exist = _bookRepository.Select.Any(r => r.Title == createBook.Title);
            if (exist)
            {
                throw new LinCmsException("图书已存在");
            }

            Book book = _mapper.Map<Book>(createBook);
            _bookRepository.Insert(book);
            return ResultDto.Success("新建图书成功");
        }

        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] CreateUpdateBookDto updateBook)
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

            return ResultDto.Success("更新图书成功");
        }
    }
}