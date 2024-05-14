using BookStore.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string AppUser { get; set; }
        public string UserId { get; set; }
        public BookStoreUser? User { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        public int? Rating { get; set; }

        [Display(Name = "Book")]
        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
