using ElectronicCard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ElectronicCard.Data
{
    public class MyAppDbContextFactory : IDesignTimeDbContextFactory<MyAppDbContext>
    {
        public MyAppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyAppDbContext>();
            optionsBuilder.UseSqlServer("Server=209.209.42.109,59228;Database=Ecard;User Id=sa;Password=Lagos!@#4;TrustServerCertificate=True;");

            return new MyAppDbContext(optionsBuilder.Options);
        }
    }
}
