using Domain.Entittes;
using Domain.Enums;
using Infrastructre.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructre.Data
{
    public static class UserSeedDate
    {
        private readonly static string adminPassword = "Admin@123";
        public static void UserSeed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BSFContext>();

            if (!context.Roles.Any())
            {
               var roles = new List<Role>
                {
                    new Role { Name = "Admin", Code = SytemRole.Admin },
                    new Role { Name = "ServiceProvider", Code = SytemRole.ServiceProvider },
                    new Role { Name = "User", Code = SytemRole.User }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var adminRoleId = context.Roles.FirstOrDefault(r => r.Code == SytemRole.Admin).Id;
                var user = new User
                {
                    Name = "Admin User",
                    Email = "admin@bsf.com",
                    PhonNumber = "00962795213723",
                    RoleId = adminRoleId
                };

                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, adminPassword);

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}
