using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; }
        public required string AppUser { get; set; }
        public string? Comment { get; set; }
        public int? Rating { get; set; }

        // Navigation properties
        public required Book Book { get; set; }
    }
}
