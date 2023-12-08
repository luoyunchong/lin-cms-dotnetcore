using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.v1.Books;
using Xunit;

namespace LinCms.Test.Repositories
{
    public class BookRepositoryTests
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookRepositoryTests(IBookRepository bookRepository, IMapper mapper) : base()
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        private Book GetBook()
        {
            CreateUpdateBookDto createBook = new CreateUpdateBookDto()
            {
                Title = Guid.NewGuid().ToString(),
                Author = Guid.NewGuid().ToString().Substring(0, 10),
                Image = Guid.NewGuid().ToString(),
                Summary = Guid.NewGuid().ToString(),
            };
            Book book = _mapper.Map<Book>(createBook);

            return book;
        }

        [Fact]
        public void Insert()
        {
            Book book = GetBook();
            Book backBook = _bookRepository.Insert(book);
            Assert.Equal(book.Author, backBook.Author);
        }

        [Fact]
        public async Task Insert2()
        {

            Book book = GetBook();
            Book backBook = await _bookRepository.InsertAsync(book);
            Assert.Equal(book.Author, backBook.Author);
        }

        [Fact]
        public void InsertList()
        {

            List<Book> listBook = new List<Book>()
            {
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
            };
            List<Book> backBooks = _bookRepository.Insert(listBook);
            Assert.Equal(listBook[0].Author, backBooks[0].Author);
        }

        [Fact]
        public async Task InsertAsyncListAsync()
        {
            List<Book> listBook = new List<Book>()
            {
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
                GetBook(),
            };
            List<Book> backBooks = await _bookRepository.InsertAsync(listBook);
            Assert.Equal(listBook[0].Author, backBooks[0].Author);

        }

        [Fact]
        public void Update()
        {
            Book book = _bookRepository.Select.First();
            book.Title = "112122123";
            int len = _bookRepository.Update(book);
            Assert.Equal(1, len);
        }


        [Fact]
        public async Task UpdateAsync()
        {
            Book book = await _bookRepository.Select.FirstAsync();
            book.Title = "123";
            int len = await _bookRepository.UpdateAsync(book);
            Assert.Equal(1, len);
        }




        [Fact]
        public void Delete()
        {
            Book book = _bookRepository.Select.First();
            _bookRepository.Delete(book);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            Book book = await _bookRepository.Select.FirstAsync();
            await _bookRepository.DeleteAsync(book);
        }



        [Fact]
        public void IsAssignableFromTest()
        {
            Type f1 = typeof(Book);
            Type f2 = typeof(IDeleteAuditEntity);
            bool f3 = typeof(Book).IsSubclassOf(typeof(IDeleteAuditEntity));
            bool f4 = typeof(IDeleteAuditEntity).IsAssignableFrom(typeof(Book));

        }

    }
}
