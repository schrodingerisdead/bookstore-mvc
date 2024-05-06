using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

        // Navigation properties
        public required ICollection<BookGenre> BookGenres { get; set; }
    }
}
