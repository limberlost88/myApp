using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BooksApi.Models;
using System.Linq;
using System;

namespace BooksApi.Controllers
{   
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly BooksContext _context;

        public BooksController(BooksContext context)
        {
            _context = context;
        }    

        [HttpGet]
        public IEnumerable<BookItem> GetAll()
        {
            return _context.BookItems.ToList();
        }

        [HttpGet("{id}", Name = "GetBook")]
        public IActionResult GetById(long id)
        {
            var item = _context.BookItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
}   
    
        [HttpPost]
        public IActionResult Create([FromBody] BookItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            /*Item must have valid rating */
            if (item.rating != 0)
                item.rating = BookType.approve_Rating(item.rating);
            /*Item must have valid genre
             Eliminates any genres added to a book that are not official book genres
            (see BookType class)*/ 
            if (item.genre != null)
                item.genre = BookType.approve_Genre(item);
            
            
            _context.BookItems.Add(item);
            /*Following changes rating according to genre preferances */
            BookType.genre_Weights(_context);
            BookType.recalculate_Rating(_context);
            _context.SaveChanges();
            return CreatedAtRoute("GetBook", new { id = item.Id }, item);
           
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] BookItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }
            /*Book must be rated from 0 to 10, any rating outside of
            this will automatically be given rating of 0 and not be considered
            for helping to find a book */
            if (item.rating != 0)
                item.rating = BookType.approve_Rating(item.rating);
            /*Eliminates any genres added to a book that are not official book genres
            see BookType class for all possible book genres*/ 
            if (item.genre != null)
                item.genre = BookType.approve_Genre(item);

            var book = _context.BookItems.FirstOrDefault(t => t.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            
            book.Name = item.Name;
            _context.BookItems.Update(book);
            BookType.genre_Weights(_context);
            BookType.recalculate_Rating(_context);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var book = _context.BookItems.FirstOrDefault(t => t.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            _context.BookItems.Remove(book);
            BookType.genre_Weights(_context);
            BookType.recalculate_Rating(_context);
            _context.SaveChanges();
            return new NoContentResult();
        }

    
    }
}