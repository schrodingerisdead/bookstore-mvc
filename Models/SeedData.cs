using BookStore.Areas.Identity.Data;
using BookStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<BookStoreUser>>();
            IdentityResult roleResult;
            IdentityResult userRoleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            //Add user Role
            var userRoleCheck = await RoleManager.RoleExistsAsync("User");
            if (!userRoleCheck) { userRoleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }
            BookStoreUser user = await UserManager.FindByEmailAsync("admin@bookstore.com");
            if (user == null)
            {
                var User = new BookStoreUser();
                User.Email = "admin@bookstore.com";
                User.UserName = "admin@bookstore.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookStoreContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<BookStoreContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
            }
        }
    }
}
