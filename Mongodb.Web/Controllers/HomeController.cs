using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mongodb.Web.Helpers;
using Mongodb.Web.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongodb.Web.Controllers
{
    public class HomeController : Controller
    {
        private IBookService _service;
        public HomeController(IBookService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var books = _service.Get();
            return View(books);
        }
        public IActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Insert(Book book)
        {
            _service.Create(book);
            TempData["Message"] = "Book added successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Update(string id)
        {          
            Book book = _service.Get(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public IActionResult Update(string id, Book bookIn)
        {
            var book = _service.Get(id);
            if (book == null)
            {
                return NotFound();
            }
            try
            {
                _service.Update(id, bookIn);
                TempData["Message"] = "Book updated successfully!";
            }
            catch
            {
                TempData["Message"] = "Error occured while updating Book!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult ConfirmDelete(string id)
        {
            Book emp = _service.ConfirmDelete(id);
            return View(emp);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var book = _service.Get(id);
            if (book == null)
            {
                return NotFound();
            }
            try
            {
                _service.Remove(book.Id);
                TempData["Message"] = "Book deleted successfully!";
            }
            catch
            {
                TempData["Message"] = "Error occuered while deleting Book!";
            }                    
            return RedirectToAction("Index");
        }

        public IActionResult Aggregate()
        {
            IEnumerable<Book> emp = new List<Book>();
            emp = _service.Aggregate();
            ViewBag.Message = "Perform Aggregate!";
            return View(emp);          
        }
    }
}
