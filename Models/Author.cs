using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Author
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get;set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [StringLength (50)]
        public string? Nationality { get; set; }
        [StringLength(50)]

        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either Male or Female")]
        public string? Gender { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }


        public ICollection<Book>? Books { get; set;}

    }
}
