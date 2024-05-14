using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Year Published")]
        public int? YearPublished { get; set; }

        [Display(Name = "Number of pages")]
        public int? NumPages { get; set; }

        [StringLength(int.MaxValue)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Publisher { get; set; }

        [StringLength(int.MaxValue)]
        [Display(Name = "Front Page")]
        public string? FrontPage { get; set; }

        [StringLength(int.MaxValue)]
        [Display(Name = "Download Url")]
        public string? DownloadUrl { get; set;}

        public ICollection<Review>? Reviews { get; set; }


        [Display(Name = "Author")]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public ICollection<BookGenre>? Genres { get; set; }

        public ICollection<UserBooks>? UserBooks { get; set; }
    }
}
