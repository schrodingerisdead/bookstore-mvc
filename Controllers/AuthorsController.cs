using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BookStore.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly BookStoreContext _context;

        public AuthorsController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index(string FirstNameSearch, string LastNameSearch, string NationalitySearch)
        {
            IQueryable<Author> authors = _context.Author.AsQueryable();
            if (!string.IsNullOrEmpty(FirstNameSearch))
            {
               authors = authors.Where(a => a.FirstName.Contains(FirstNameSearch));
            }

            if (!string.IsNullOrEmpty(LastNameSearch))
            {
                authors = authors.Where(a => a.LastName.Contains(LastNameSearch));
            }

            if (!string.IsNullOrEmpty(NationalitySearch))
            {
                authors = authors.Where(a => a.Nationality.Contains(NationalitySearch));
            }

            var authorVM = new AuthorsViewModel
            {
                Authors = await authors.ToListAsync()
            };
            return View(authorVM);
            /*return _context.Author != null ? 
                        View(await _context.Author.ToListAsync()) :
                        Problem("Entity set 'BookStoreContext.Author'  is null.");*/
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Author == null)
            {
                return NotFound();
            }

            IQueryable<Book> books = _context.Book.AsQueryable();

            var author = await _context.Author
                .Include( a => a.Books)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            books = books.Include(b => b.Author);
            books = books.Where(b => b.AuthorId == id);

            var averageRatings = await _context.Review
                .GroupBy(r => r.BookId)
                .Select(g => new {
                    BookId = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToDictionaryAsync(x => x.BookId, x => x.AverageRating);

            ViewBag.AverageRatings = averageRatings;


            return View(author);
        }

        // GET: Authors/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [Authorize(Roles = "Admin")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,BirthDate,Nationality,Gender")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Author == null)
            {
                return NotFound();
            }

            var author = await _context.Author.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        [Authorize(Roles = "Admin")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthDate,Nationality,Gender")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Author == null)
            {
                return NotFound();
            }

            var author = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Author == null)
            {
                return Problem("Entity set 'BookStoreContext.Author'  is null.");
            }
            var author = await _context.Author.FindAsync(id);
            if (author != null)
            {
                _context.Author.Remove(author);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
          return (_context.Author?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
