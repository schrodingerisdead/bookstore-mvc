using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BookStoreUser class
public class BookStoreUser : IdentityUser
{
    public ICollection<UserBooks>? UserBooks { get; set; }
    public ICollection<Review>? Reviews { get; set; }

}

