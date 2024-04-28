using GetImagesApi.Constants;
using GetImagesApi.Data.Entities;
using GetImagesApi.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GetImagesApi.Data;

public static class SeederDB
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MyAppContext>();
            context.Database.Migrate();

            var userManager = scope.ServiceProvider
                                .GetRequiredService<UserManager<UserEntity>>();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<RoleEntity>>();

            #region Seed Roles and Users

            if (!context.Roles.Any())
            {
                foreach (var role in Roles.All)
                {
                    var result = roleManager.CreateAsync(new RoleEntity
                    {
                        Name = role
                    }).Result;
                }
            }

            var userId = 0L;
            if (!context.Users.Any())
            {
                UserEntity user = new()
                {
                    FirstName = "Юхим",
                    LastName = "Капот",
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                };
                var result = userManager.CreateAsync(user, "123456")
                    .Result;
                if (result.Succeeded)
                {
                    userId = user.Id;
                    result = userManager
                        .AddToRoleAsync(user, Roles.Admin)
                        .Result;
                }
            }

            #endregion

            if (!context.Categories.Any())
            {
                var kovbasy = new CategoryEntity
                {
                    Name = "Ковбаси",
                    Description = "Хороші і довго ковбаси",
                    UserId = userId,
                };
                var vsutiy = new CategoryEntity
                {
                    Name = "Взуття",
                    Description = "Гарне взуття із гарнатуєю 5 років." +
                    "Можна нирять під воду.",
                    UserId = userId,
                };
                context.Categories.Add(kovbasy);
                context.Categories.Add(vsutiy);
                context.SaveChanges();
            }
        }
    }
}
