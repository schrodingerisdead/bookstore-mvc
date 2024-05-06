using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
     public class BookGenre
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; }
        public required Book Book { get; set; }

        public int GenreId { get; set; }
        public required Genre Genre { get; set; }
    }
}
