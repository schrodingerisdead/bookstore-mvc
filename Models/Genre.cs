using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Genre")]
        public string GenreName { get; set; }

        public ICollection<BookGenre>? Books { get; set; }
    }
}
