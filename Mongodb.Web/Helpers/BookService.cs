using Mongodb.Web.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
        void Remove(string id);
        IEnumerable<Aggregate> Aggregate();
    }

    public class BookService : IBookService
    {
        private readonly IMongoCollection<Book> _books;
        private readonly MongoClient _client;

        public BookService(IBookstoreDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            var database = _client.GetDatabase(settings.DatabaseName);
            _books = database.GetCollection<Book>(settings.BooksCollectionName);
        }

        public List<Book> Get()
        {
            var group = new BsonDocument
                {
                    { "$group",
                        new BsonDocument
                            {
                                { "_id", new BsonDocument
                                             {
                                                 {
                                                     "MyPrice","$Price"
                                                 }
                                             }
                                },
                                {
                                    "Count", new BsonDocument
                                                 {
                                                     {
                                                         "$sum", "$Count"
                                                     }
                                                 }
                                }
                            }
                  }
                };

            //var pipeline = new[] { group };
            //var result = _books.Aggregate<Book>(pipeline);
            //var r = result.ToList().Find(book => true).();
            //var matchingExamples = result.ToList()
            //    .Select(x => x.ToList())
            //    .ToList();


            //var aggregate = _books.Aggregate()
            //                           .Group(new BsonDocument { { "price", "$token" }, { "count", new BsonDocument("$sum", 1) } })
            //                           .Sort(new BsonDocument { { "count", -1 } })
            //                           .Limit(10);

            return _books.Find(book => true).ToList();
        }

        public Book Get(string id) =>
            _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book bookIn) 
        {
            _books.ReplaceOne(book => book.Id == id, bookIn);
        }

        public void Remove(string id) 
        {
            _books.DeleteOne(book => book.Id == id);
        }

        public IEnumerable<Aggregate> Aggregate()
        {
            var result = _books.Aggregate()            
              .Group(key => key.BookName,
               value => new {
                   BookName = value.Key,
                   Total = value.Select(x => x.Price).Max()               
               })
              .Sort(new BsonDocument { { "Price", -1 } }).ToList();

            IEnumerable<Aggregate> bookResult = BsonSerializer.Deserialize<List<Aggregate>>(result.ToJson());

            return bookResult;
        }
    }
}