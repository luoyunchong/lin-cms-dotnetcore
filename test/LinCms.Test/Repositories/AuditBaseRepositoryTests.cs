using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Test.Controller;
using LinCms.Web;
using LinCms.Web.Models.v1.Books;
using LinCms.Web.Repositories;
using LinCms.Zero.Domain;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Repositories
{
    public class AuditBaseRepositoryTests : BaseControllerTests
    {
        private readonly IMapper _mapper;
        private readonly AuditBaseRepository<Book> _bookRepository;

        public AuditBaseRepositoryTests()
        {
            _bookRepository = serviceProvider.GetService<AuditBaseRepository<Book>>();
            _mapper = serviceProvider.GetService<IMapper>();
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

            Assert.Equal(backBooks[0].Author, backBooks[0].Author);
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

            Assert.Equal(backBooks[0].Author, backBooks[0].Author);
        }

        [Fact]
        public void Update()
        {
            Book book = GetBook();
            book.Id = 5;
            book.Title = "112122123";
            int len = _bookRepository.Update(book);

            Assert.Equal(len, 1);
        }


        [Fact]
        public async Task UpdateAsync()
        {
            Book book = GetBook();
            book.Id = 7;
            _bookRepository.Attach(book);
            book.Title = "123";
            int len = await _bookRepository.UpdateAsync(book);

            Assert.Equal(len, 1);
        }

        [Fact]
        public void UpdateList()
        {
            var b1 = GetBook();
            var b2 = GetBook();

            b1.Id = 8;
            b2.Id = 9;

            List<Book> books = new List<Book>()
            {
                b1,b2
            };
            int len = _bookRepository.Update(books);

            Assert.Equal(len, 2);
        }
        [Fact]
        public async Task UpdateListAsync()
        {
            var b1 = GetBook();
            var b2 = GetBook();
            var b3 = GetBook();
            var b4 = GetBook();

            b1.Id = 8;
            b2.Id = 9;
            b3.Id = 10;
            b4.Id = 11;

            b1.Title = "123";

            List<Book> books = new List<Book>()
            {
                b1,b2,b3,b4
            };

            _bookRepository.Attach(books);
            int len = await _bookRepository.UpdateAsync(books);

            Assert.Equal(len, 4);
        }

        [Fact]
        public void Delete()
        {
            _bookRepository.Delete(new Book() { Id = 6 });
        }

        [Fact]
        public async Task DeleteAsync()
        {
            await _bookRepository.DeleteAsync(new Book() { Id = 7 });
        }

        [Fact]
        public void DeleteList()
        {
            List<Book> books = new List<Book>()
            {
                new Book(){Id=10},
                new Book(){Id=8} ,
                new Book(){Id=9}
                };
            _bookRepository.Delete(books);
        }

        [Fact]
        public async Task DeleteAsyncList()
        {
            List<Book> books = new List<Book>()
            {
                new Book(){Id=11},
                new Book(){Id=12} ,
                new Book(){Id=13}
            };
            await _bookRepository.DeleteAsync(books);
        }

        [Fact]
        public void DeleteWhere()
        {
            _bookRepository.Delete(r => r.Id > 13 && r.Id < 15);
        }

        [Fact]
        public async Task DeleteAsyncWhere()
        {
            await _bookRepository.DeleteAsync(r => r.Id >= 15 && r.Id < 18);
        }

        [Fact]
        public void f()
        {
            var f = typeof(Book);
            var f2 = typeof(ISoftDeleteAduitEntity);
            var f3 = typeof(Book).IsSubclassOf(typeof(ISoftDeleteAduitEntity));
            var f4 = typeof(ISoftDeleteAduitEntity).IsAssignableFrom(typeof(Book));

        }
    }
}
