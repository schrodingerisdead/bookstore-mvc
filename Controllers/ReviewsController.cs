using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using BookStore.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookStore.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly UserManager<BookStoreUser> _userManager;


        public ReviewsController(BookStoreContext context, UserManager<BookStoreUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Reviews
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var username = user?.UserName;

            var bookStoreContext = _context.Review.Include(r => r.Book).Where(r => r.AppUser == username);
            return View(await bookStoreContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "User")]

        public async Task<IActionResult> CreateAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            var books = _context.UserBooks
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.Book)
                .ToList();

            ViewData["BookId"] = new SelectList(books, "Id", "Title");
            return View();
        }

        // POST: Reviews/Create
        [Authorize(Roles = "User")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUser,Comment,Rating,BookId, UserId")] Review review)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var username = user?.UserName;

            ModelState.Remove("UserId");
            ModelState.Remove("AppUser");
            if (ModelState.IsValid)
            {
                review.UserId = userId;
                review.AppUser = username;
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var books = _context.UserBooks
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.Book)
                .ToList();
            ViewData["BookId"] = new SelectList(/*_context.Book*/ books, "Id", "Title", review.BookId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            if (review.UserId != userId)
            {
                return Forbid();
            }

            var books = _context.UserBooks
                .Where(ub => ub.UserId == userId)  
                .Select(ub => ub.Book)
                .ToList();

            ViewData["BookId"] = new SelectList(/*_context.Book*/ books, "Id", "Title", review.BookId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        [Authorize(Roles = "User")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUser,Comment,Rating,BookId")] Review review)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var username = user?.UserName;

            if (id != review.Id)
            {
                return NotFound();
            }
            //if (!ModelState.IsValid)
           //{
    // Handle invalid model state
              //  return BadRequest(ModelState);
            //}

            ModelState.Remove("AppUser");
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                try
                {
                    review.AppUser = username;
                    review.UserId = userId;
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            var books = _context.UserBooks
               .Where(ub => ub.UserId == userId)
               .Select(ub => ub.Book)
               .ToList();

            ViewData["BookId"] = new SelectList(/*_context.Book*/ books, "Id", "Title", review.BookId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != userId)
            {
                return Forbid();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [Authorize(Roles = "User")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Review == null)
            {
                return Problem("Entity set 'BookStoreContext.Review'  is null.");
            }
            var review = await _context.Review.FindAsync(id);
            if (review != null)
            {
                _context.Review.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
          return (_context.Review?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
