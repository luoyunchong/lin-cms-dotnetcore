using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using LinCms.Application.Contracts.v1.Books;
using LinCms.Application.Contracts.v1.Books.Dtos;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;
using LinCms.Core.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Repositories
{
    public class BookRepositoryTests : BaseLinCmsTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly IUnitOfWork _unitOfWork;

        public BookRepositoryTests() : base()
        {
            _bookRepository = GetRequiredService<IBookRepository>();
            _mapper = GetRequiredService<IMapper>();
            _freeSql = GetRequiredService<IFreeSql>();
            _unitOfWork = GetRequiredService<IUnitOfWork>();
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
            _unitOfWork.Commit();
            Assert.Equal(book.Author, backBook.Author);
        }

        [Fact]
        public async Task Insert2()
        {

            Book book = GetBook();
            Book backBook = await _bookRepository.InsertAsync(book);
            _unitOfWork.Commit();
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
            _unitOfWork.Commit();
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
            using (_unitOfWork)
            {
                //_unitOfWork.GetOrBeginTransaction();
                List<Book> backBooks = await _bookRepository.InsertAsync(listBook);
                _unitOfWork.Commit();
                Assert.Equal(listBook[0].Author, backBooks[0].Author);

            }

        }

        [Fact]
        public void Update()
        {
            Book book = _bookRepository.Select.First();
            book.Title = "112122123";
            int len = _bookRepository.Update(book);
            _unitOfWork.Commit();
            Assert.Equal(1, len);
        }


        [Fact]
        public async Task UpdateAsync()
        {
            Book book = await _bookRepository.Select.FirstAsync();
            book.Title = "123";
            int len = await _bookRepository.UpdateAsync(book);
            _unitOfWork.Commit();
            Assert.Equal(1, len);
        }




        [Fact]
        public void Delete()
        {
            Book book = _bookRepository.Select.First();
            _bookRepository.Delete(book);
            _unitOfWork.Commit();
        }

        [Fact]
        public async Task DeleteAsync()
        {
            Book book = await _bookRepository.Select.FirstAsync();
            await _bookRepository.DeleteAsync(book);
            _unitOfWork.Commit();
        }



        [Fact]
        public void IsAssignableFromTest()
        {
            Type f = typeof(Book);
            Type f2 = typeof(IDeleteAduitEntity);
            bool f3 = typeof(Book).IsSubclassOf(typeof(IDeleteAduitEntity));
            bool f4 = typeof(IDeleteAduitEntity).IsAssignableFrom(typeof(Book));

        }

    }
}
