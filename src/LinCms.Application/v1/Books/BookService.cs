using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Data;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.Extensions;

namespace LinCms.v1.Books;

public class BookService(IAuditBaseRepository<Book> bookRepository) : ApplicationService, IBookService
{
    public async Task CreateAsync(CreateUpdateBookDto createBook)
    {
        bool exist = bookRepository.Select.Any(r => r.Title == createBook.Title);
        if (exist)
        {
            throw new LinCmsException("图书已存在");
        }

        Book book = Mapper.Map<Book>(createBook);
        await bookRepository.InsertAsync(book);
    }

    public Task DeleteAsync(long id)
    {
        return bookRepository.DeleteAsync(new Book { Id = id });
    }

    public async Task<BookDto> GetAsync(long id)
    {
        Book book = await bookRepository.Select.Where(a => a.Id == id).ToOneAsync();
        return Mapper.Map<BookDto>(book);
    }
    public async Task<List<BookDto>> GetListAsync()
    {
        List<BookDto> items = (await bookRepository.Select.OrderByDescending(r => r.Id).ToListAsync()).Select(r => Mapper.Map<BookDto>(r)).ToList();

        return items;
    }

    public async Task<PagedResultDto<BookDto>> GetPageListAsync(PageDto pageDto)
    {
        List<BookDto> items = (await bookRepository.Select.OrderByDescending(r => r.Id)
            .ToPagerListAsync(pageDto, out long count)).Select(r => Mapper.Map<BookDto>(r)).ToList();

        return new PagedResultDto<BookDto>(items, count);
    }

    public async Task UpdateAsync(long id, CreateUpdateBookDto updateBook)
    {
        Book book = await bookRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (book == null)
        {
            throw new LinCmsException("没有找到相关书籍");
        }

        bool exist = await bookRepository.Select.AnyAsync(r => r.Title == updateBook.Title && r.Id != id);
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
        Mapper.Map(updateBook, book);

        await bookRepository.UpdateAsync(book);
    }
}