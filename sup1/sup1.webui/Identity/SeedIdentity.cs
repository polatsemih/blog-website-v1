using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace sup1.webui.Identity
{
    public static class SeedIdentity
    {
        public static async Task Seed(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            var roles = configuration.GetSection("Accounts:Roles").GetChildren().Select(role => role.Value).ToArray();
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var users = configuration.GetSection("Accounts:Users");
            foreach (var user in users.GetChildren())
            {
                var email = user.GetValue<string>("email");
                var password = user.GetValue<string>("password");
                var username = user.GetValue<string>("username");
                var role = user.GetValue<string>("role");
                var firstname = user.GetValue<string>("firstname");
                var lastname = user.GetValue<string>("lastname");
                var imageUrl = user.GetValue<string>("imageUrl");

                if (await userManager.FindByNameAsync(username) == null)
                {
                    var newuser = new User()
                    {
                        Email = email,
                        EmailConfirmed = true,
                        UserName = username,
                        FirstName = firstname,
                        LastName = lastname,
                        ImageUrl = imageUrl
                    };

                    var result = await userManager.CreateAsync(newuser, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newuser, role);
                    }
                }
            }
        }
    }
}