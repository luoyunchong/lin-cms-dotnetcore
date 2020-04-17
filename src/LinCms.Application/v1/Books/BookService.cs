using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Cms.Books;
using LinCms.Application.Contracts.v1.Books.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinCms.Application.Cms.Users
{

    public class BookService : IBookService
    {
        private readonly IAuditBaseRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        public BookService(IAuditBaseRepository<Book> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateUpdateBookDto createBook)
        {
            bool exist = _bookRepository.Select.Any(r => r.Title == createBook.Title);
            if (exist)
            {
                throw new LinCmsException("图书已存在");
            }

            Book book = _mapper.Map<Book>(createBook);
            await _bookRepository.InsertAsync(book);
        }

        public async Task DeleteAsync(long id)
        {
            await _bookRepository.DeleteAsync(new Book { Id = id });
        }

        public async Task<BookDto> GetAsync(long id)
        {
            Book book = await _bookRepository.Select.Where(a => a.Id == id).ToOneAsync();
            return _mapper.Map<BookDto>(book);
        }

        public async Task<PagedResultDto<BookDto>> GetListAsync(PageDto pageDto)
        {
            List<BookDto> items = (await _bookRepository.Select.OrderByDescending(r => r.Id)
                        .ToPagerListAsync(pageDto, out long count)).Select(r => _mapper.Map<BookDto>(r)).ToList();

            return new PagedResultDto<BookDto>(items, count);
        }

        public async Task UpdateAsync(long id, CreateUpdateBookDto updateBook)
        {
            Book book = _bookRepository.Select.Where(r => r.Id == id).ToOne();
            if (book == null)
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
            _mapper.Map(updateBook, book);

            await _bookRepository.UpdateAsync(book);
        }
    }
}