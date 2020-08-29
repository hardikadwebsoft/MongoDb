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
        void Remove(Book bookIn);
        void Remove(string id);
        Book ConfirmDelete(string id);
        IEnumerable<Book> Aggregate();
    }

    public class BookService : IBookService
    {
        private readonly IMongoCollection<Book> _books;
        private readonly MongoClient _client;

        public BookService(IBookstoreDatabaseSettings settings, MongoClient client)
        {
            _client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _books = database.GetCollection<Book>(settings.BooksCollectionName);
            _client = client;
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
            using (var session = _client.StartSession())
            {
                // Begin transaction
                session.StartTransaction();

                try
                {
                    _books.InsertOne(book);
                    session.CommitTransaction();
                }
                catch(Exception ex)
                {
                    session.AbortTransaction();
                }
                
                return book;
            }
        }

        public void Update(string id, Book bookIn) 
        {
            using (var session = _client.StartSession())
            {
                session.StartTransaction();

                try
                {
                    _books.ReplaceOne(book => book.Id == id, bookIn);
                    session.CommitTransaction();
                }
                catch (Exception ex)
                {
                    session.AbortTransaction();
                }
            }

            //_books.ReplaceOne(book => book.Id == id, bookIn);
        }


        public void Remove(Book bookIn) 
        {
            using (var session = _client.StartSession())
            {
                session.StartTransaction();

                try
                {
                    _books.DeleteOne(book => book.Id == bookIn.Id);
                    session.CommitTransaction();
                }
                catch (Exception ex)
                {
                    session.AbortTransaction();
                }
            }
            //_books.DeleteOne(book => book.Id == bookIn.Id);
        }

        public Book ConfirmDelete(string id) =>
         _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public void Remove(string id) 
        {
            using (var session = _client.StartSession())
            {
                session.StartTransaction();

                try
                {
                    _books.DeleteOne(book => book.Id == id);
                    session.CommitTransaction();
                }
                catch (Exception ex)
                {
                    session.AbortTransaction();
                }
            }
            //_books.DeleteOne(book => book.Id == id);
        }
            

        public IEnumerable<Book> Aggregate()
        {
          
            var result = _books.Aggregate()
               .Match(x => x.Category == "book")                                                   
               .Group(BsonDocument.Parse("{ '_id':'$Name'}"))
               .Sort(new BsonDocument {{ "Price", -1 } }).ToList();

            IEnumerable<Book> usersOfInterestList = BsonSerializer.Deserialize<List<Book>>(result.ToJson());
            
            return usersOfInterestList;          
        }     
    }
}