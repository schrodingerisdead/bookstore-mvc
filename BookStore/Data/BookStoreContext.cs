using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext (DbContextOptions<BookStoreContext> options)
            : base(options)
        {
        }

        public DbSet<BookStore.Models.Book> Book { get; set; } = default!;
        public DbSet<BookStore.Models.Author> Author { get; set; } = default!;
        public DbSet<BookStore.Models.BookGenre> BookGenre { get; set; } = default!;
        public DbSet<BookStore.Models.Genre> Genre { get; set; } = default!;
        public DbSet<BookStore.Models.UserBooks> UserBooks { get; set; } = default!;
        public DbSet<BookStore.Models.Review> Review { get; set; } = default!;
    }
}
