using ElectronicCard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronicCard.Data
{
    public class MyAppDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor to pass options to the base class
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options) { }

        // DbSets for different entities representing tables in the database
        public DbSet<Admin> Admins { get; set; }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ProvinceUser> ProvinceUsers { get; set; }

        public DbSet<ChairmanImages> ChairmanImages { get; set; }

        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Savings> Savings { get; set; }
        public DbSet<Member> Members { get; set; }

        public DbSet<Province> Provinces { get; set; }

        // Fluent API configuration ()
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Group>()
                    .HasOne(g => g.Province)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(g => g.Province_ID);


            modelBuilder.Entity<Group>()
                .HasOne(g => g.ProvinceUser)
                .WithMany()
                .HasForeignKey(g => g.Province_User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserGroup>()
           .HasKey(ug => new { ug.UserId, ug.GroupId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.GroupUsers)
                .HasForeignKey(ug => ug.GroupId);


        }

    }
}
