using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class UserBooks
    {
        [Key]
        public int Id { get; set; }
        public string AppUser { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
