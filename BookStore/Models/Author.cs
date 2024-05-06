using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }

        // Navigation properties
        public ICollection<Book> Books { get; set; }
    }
}
