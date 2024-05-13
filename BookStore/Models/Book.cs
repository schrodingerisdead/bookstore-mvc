using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BookStore.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100), Display(Name ="Title")]
        public string Title { get; set; }
        [Required, DataType(DataType.Date), Display(Name = "Date Published") ]
        public DateTime? YearPublished { get; set; }
        public int NumPages { get; set; }
        [Required, MaxLength(200), Display(Name ="Description")]
        public string? Description { get; set; }

        [MaxLength(50), Display(Name = "Publisher") ]
        public string? Publisher { get; set; }
        public string? FrontPage { get; set; }
        [Display(Name ="Download Url")]
        public string? DownloadUrl { get; set; }

        // Navigation properties
        [Required]
        public int? AuthorId { get; set; }
        [Required]
        public Author? Author { get; set; }

        public ICollection<UserBooks>? UserBooks { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<BookGenre>? BookGenres { get; set; }
    }
}
