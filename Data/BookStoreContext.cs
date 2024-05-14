using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookStore.Areas.Identity.Data;
using BookStore.Models;

namespace BookStore.Data
{
    public class BookStoreContext : IdentityDbContext<BookStoreUser>
    {
        public BookStoreContext (DbContextOptions<BookStoreContext> options)
            : base(options)
        {
        }

        public DbSet<BookStore.Models.Author> Author { get; set; } = default!;

        public DbSet<BookStore.Models.Book>? Book { get; set; }

        public DbSet<BookStore.Models.Genre>? Genre { get; set; }

        public DbSet<BookStore.Models.UserBooks>? UserBooks { get; set; }

        public DbSet<BookStore.Models.BookGenre>? BookGenre { get; set;}

        public DbSet<BookStore.Models.Review>? Review { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*
            modelBuilder.Entity<BookGenre>()
                .HasOne<Genre>(p => p.Genre)
                .WithMany(p => p.Books)
                .HasForeignKey(p => p.GenreId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder.Entity<BookGenre>()
                .HasOne<Book>(p => p.Book)
                .WithMany(p => p.Genres)
                .HasForeignKey(p => p.BookId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder.Entity<Book>()
                .HasOne<Author>(p => p.Author)
                .WithMany(p => p.Books)
                .HasForeignKey(p => p.AuthorId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder.Entity<Review>()
                .HasOne<Book>( p => p.Book)
                .WithMany(p => p.Reviews)
                .HasForeignKey(p => p.BookId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder.Entity<UserBooks>()
                .HasOne<Book>(p => p.Book)
                .WithMany(p => p.UserBooks)
                .HasForeignKey(p => p.BookId)
                .HasPrincipalKey(p => p.Id); */

           /* modelBuilder.Entity<UserBooks>()
            .HasKey(ub => new { ub.BookId, ub.AppUser });

            modelBuilder.Entity<UserBooks>()
                .HasOne(ub => ub.Book)
                .WithMany(b => b.UserBooks)
                .HasForeignKey(ub => ub.BookId);

            modelBuilder.Entity<UserBooks>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBooks)
                .HasForeignKey(ub => ub.AppUser);*/

        }
    }
}
