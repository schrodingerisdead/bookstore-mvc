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
using BookStore.Interfaces;
using BookStore.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BookStore.Areas.Identity.Data;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreContext _context;
        readonly IBufferedFileUploadService _bufferedFileUploadService;
        readonly IStreamFileUploadService _streamFileUploadService;
        private readonly UserManager<BookStoreUser> _userManager;



        public BooksController(BookStoreContext context, IStreamFileUploadService streamFileUploadService, IBufferedFileUploadService bufferedFileUploadService, UserManager<BookStoreUser> userManager)
        {
            _context = context;
            _streamFileUploadService = streamFileUploadService;
            _bufferedFileUploadService = bufferedFileUploadService;
            _userManager = userManager;

        }


        // GET: Books
        public async Task<IActionResult> Index(string bookGenre, string searchString)
        {
            IQueryable<Book> books = _context.Book.AsQueryable();
            IQueryable<string> genreQuery = _context.Genre.OrderBy(b => b.Id).Select(b => b.GenreName);

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(bookGenre))
            {

                books = books.Where(b => b.Genres.Any(bg => bg.Genre.GenreName == bookGenre));
            }

            books = books.Include(b => b.Author);

            var averageRatings = await _context.Review
                .GroupBy(r => r.BookId)
                .Select(g => new {
                    BookId = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToDictionaryAsync(x => x.BookId, x => x.AverageRating);

            ViewBag.AverageRatings = averageRatings;


            var bookGenreVM = new BookGenreViewModel
            {
                Genres = new SelectList(await genreQuery.ToListAsync()),
                Books = await books.ToListAsync()
            };
            return View(bookGenreVM);
        }

        // GET: GetMyBooks
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyBooks(string bookGenre, string searchString)
        {
            string message = Request.Query["message"];
            ViewBag.Message = message;

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            IQueryable<Book> books = _context.Book
                .Join(_context.UserBooks, b => b.Id, ub => ub.BookId, (b, ub) => new { Book = b, UserBook = ub })
                .Where(x => x.UserBook.UserId == userId)
                .Select(x => x.Book); IQueryable<string> genreQuery = _context.Genre.OrderBy(b => b.Id).Select(b => b.GenreName);

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(bookGenre))
            {

                books = books.Where(b => b.Genres.Any(bg => bg.Genre.GenreName == bookGenre));
            }

            books = books.Include(b => b.Author);

            var averageRatings = await _context.Review
                .GroupBy(r => r.BookId)
                .Select(g => new {
                    BookId = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToDictionaryAsync(x => x.BookId, x => x.AverageRating);

            ViewBag.AverageRatings = averageRatings;


            var bookGenreVM = new BookGenreViewModel
            {
                Genres = new SelectList(await genreQuery.ToListAsync()),
                Books = await books.ToListAsync()
            };
            return View(bookGenreVM);
        }


        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            ViewBag.HasPurchased = false;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id; 
            if (userId != null)
            {
                var userBooks = await _context.UserBooks.Where(ub => ub.UserId == userId && ub.BookId == id).ToListAsync();
                if (userBooks.Any())
                {
                    ViewBag.HasPurchased = true;
                }
            }


            var averageRating = await _context.Review
                .Where(r => r.BookId == book.Id)
                .AverageAsync(r => r.Rating);

            ViewBag.AverageRating = averageRating;

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            var genres = _context.Genre.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.GenreName
            }).ToList();

            ViewData["Genres"] = genres;
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName");
            return View();
        }

        // POST: Books/Create
        [Authorize(Roles = "Admin")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, IFormFile pdffile, [Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book, int[] selectedGenres)
        {
            ModelState.Remove("file");
            ModelState.Remove("pdffile");
            if (ModelState.IsValid)
            {
                try
                {
                    book.FrontPage = await _bufferedFileUploadService.UploadFile(file);
                    book.DownloadUrl = await _bufferedFileUploadService.UploadFile(pdffile);
                    if (!string.IsNullOrEmpty(book.FrontPage) && !string.IsNullOrEmpty(book.DownloadUrl))
                    {
                        ViewBag.Message = "File Upload Successful";
                    }
                    else
                    {
                        ViewBag.Message = "File Upload Failed";
                    }
                }
                catch (Exception ex)
                {
                    //Log ex
                    ViewBag.Message = "File Upload Failed";
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                if (selectedGenres != null)
                {
                    foreach (var genreId in selectedGenres)
                    {
                        var genreBook = new BookGenre
                        {
                            GenreId = genreId,
                            BookId = book.Id
                        };

                        _context.Add(genreBook);
                    }

                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }



            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var bookGenres = _context.BookGenre
                .Where(bg => bg.BookId == book.Id)
                .Select(bg => bg.GenreId)
                .ToList();

            var genres = _context.Genre.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.GenreName,
                Selected = bookGenres.Contains(g.Id) // Da se selektira genre-ot koj e kaj book
            }).ToList();

            ViewData["Genres"] = genres;
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(book);
        }


        // POST: Books/Edit/5
        [Authorize(Roles = "Admin")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile fileimg, IFormFile filepdf, [Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book, int[] selectedGenres)
        {

            if (id != book.Id)
            {
                return NotFound();
            }
            ModelState.Remove("filepdf");
            ModelState.Remove("fileimg");
            if (ModelState.IsValid)
            {
                var oldBook = await _context.Book.FindAsync(book.Id);
                if (fileimg?.Length > 0)
                {
                    try
                    {
                        book.FrontPage = await _bufferedFileUploadService.UploadFile(fileimg);
                        if (!string.IsNullOrEmpty(book.FrontPage))
                        {
                            ViewBag.Message = "File Upload Successful";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log ex
                        ViewBag.Message = "File Upload Failed";
                    }


                }
                else
                {
                    book.FrontPage = oldBook.FrontPage;

                }

                if (filepdf?.Length > 0)
                {
                    try
                    {
                        book.DownloadUrl = await _bufferedFileUploadService.UploadFile(filepdf);
                        if (!string.IsNullOrEmpty(book.DownloadUrl))
                        {
                            ViewBag.Message = "File Upload Successful";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log ex
                        ViewBag.Message = "File Upload Failed";
                    }
                }


                else
                {
                    book.DownloadUrl = oldBook.DownloadUrl;

                }

                try
                {
                    _context.Entry(oldBook).State = EntityState.Detached;
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    if (selectedGenres != null)
                    {
                        // brishenje
                        var oldBookGenres = _context.BookGenre.Where(bg => bg.BookId == book.Id);
                        _context.BookGenre.RemoveRange(oldBookGenres);

                        // dodavanje
                        foreach (var genreId in selectedGenres)
                        {
                            var genreBook = new BookGenre
                            {
                                GenreId = genreId,
                                BookId = book.Id
                            };

                            _context.Update(genreBook);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'BookStoreContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var username = user?.UserName;

            /*if (!ModelState.IsValid)
            {
                // Handle invalid model state
                return BadRequest(ModelState);
            }*/


            //if (ModelState.IsValid)
            //{
             
               var userbook = new UserBooks
                {
                    AppUser = username,
                    UserId = userId,
                    BookId = book.Id
                };
                _context.Add(userbook);
                await _context.SaveChangesAsync();


            ViewBag.Message = "You bought book successfully";
            //return RedirectToAction(nameof(GetMyBooks));
            return RedirectToAction(nameof(GetMyBooks), new { message = "You bought book successfully" });

            //}


            //ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            //return View(book);
            //return RedirectToAction("GetMyBooks", "Books");

        }

    }
}
