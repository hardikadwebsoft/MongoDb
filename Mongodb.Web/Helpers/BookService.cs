using Mongodb.Web.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mongodb.Web.Helpers
{
    public interface IBookService
    {
        List<Book> Get();
        Book Get(string id);
        Book Create(Book book);
        void Update(string id, Book bookIn);
        void Remove(Book bookIn);
        void Remove(string id);
        Book ConfirmDelete(string id);
    }

    public class BookService : IBookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _books = database.GetCollection<Book>(settings.BooksCollectionName);
        }

        public List<Book> Get() =>
            _books.Find(book => true).ToList();

        public Book Get(string id) =>
            _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book bookIn) =>
            _books.ReplaceOne(book => book.Id == id, bookIn);

        public void Remove(Book bookIn) =>
            _books.DeleteOne(book => book.Id == bookIn.Id);

        public Book ConfirmDelete(string id) =>
         _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public void Remove(string id) =>
            _books.DeleteOne(book => book.Id == id);
    }
}
