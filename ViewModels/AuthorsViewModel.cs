using BookStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.ViewModels
{
    public class AuthorsViewModel
    {
        public IList<Author> Authors { get; set; }
        public string FirstNameSearch { get; set; }
        public string LastNameSearch { get; set; }

        public string NationalitySearch { get; set; }
    }
}
