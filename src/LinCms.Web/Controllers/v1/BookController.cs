using System;
using System.Collections.Generic;
using AutoMapper;
using FreeSql;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Models.v1.Books;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        public BookController(IFreeSql freeSql, IMapper mapper)
        {
            _freeSql = freeSql;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除图书", "图书")]
        public ResultDto DeleteBook(int id)
        {
            _freeSql.Delete<Book>(id).ExecuteAffrows();
            return ResultDto.Success();
        }

        [HttpGet]
        public List<Book> Get()
        {
            return _freeSql.Select<Book>().ToList();
        }

        [HttpGet("{id}", Name = "Get")]
        public BookDto Get(int id)
        {
            Book book = _freeSql.Select<Book>().Where(a => a.Id == id).ToOne();
            return _mapper.Map<BookDto>(book);
        }

        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateBookDto createBook)
        {
            bool exist = _freeSql.Select<Book>().Any(r => r.Title == createBook.Title);
            if (exist)
            {
                throw new LinCmsException("图书已存在");
            }

            Book book = _mapper.Map<Book>(createBook);
            _freeSql.Insert(book).ExecuteAffrows();
            return ResultDto.Success("新建图书成功");
        }

        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] CreateUpdateBookDto updateBook)
        {
            //这种找不到逻辑，我一般不写的。
            bool bookExist = _freeSql.Select<Book>().Any(r => r.Id == id);
            if (!bookExist)
            {
                throw new LinCmsException("没有找到相关书籍");
            }

            bool exist = _freeSql.Select<Book>().Any(r => r.Title == updateBook.Title && r.Id != id);
            if (exist)
            {
                throw new LinCmsException("图书已存在");
            }

            _freeSql.Update<Book>(id).Set(a => new Book()
            {
                Title = updateBook.Title,
                Author = updateBook.Author,
                Image = updateBook.Image,
                Summary = updateBook.Summary,
            }).ExecuteAffrows();

            return ResultDto.Success("更新图书成功");
        }
    }
}