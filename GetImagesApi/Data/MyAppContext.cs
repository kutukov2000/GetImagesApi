using GetImagesApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GetImagesApi.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options)
        {

        }

        public DbSet<CategoryEntity> Categories { get; set; }
    }
}
