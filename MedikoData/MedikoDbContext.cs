
using MedikoData.Configurations;
using MedikoData.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedikoData
{
    public class MedikoDbContext : IdentityDbContext<AppUser>
    {
        public MedikoDbContext() { }
        public MedikoDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Log> Logs { get; set; } = null!;
        public DbSet<LogBook> LogBooks { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            string adminRoleGUID = Guid.NewGuid().ToString();
            string patientRoleGUID = Guid.NewGuid().ToString();
            string adminGUID = Guid.NewGuid().ToString();


            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleGUID,
                    Name = "Admin",
                    NormalizedName = "Administrator"
                },
                new IdentityRole
                {
                    Id = patientRoleGUID,
                    Name = "Patient",
                    NormalizedName = "Patient"
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Doktor",
                    NormalizedName = "Doktor"
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Editor",
                    NormalizedName = "Editor"
                }
                );


            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new LogBookConfiguration());
            builder.ApplyConfiguration(new LogConfiguration());


            // - - - Create Admin AppUser - - -
            AppUser admin = new AppUser
            {
                Id = adminGUID,
                UserName = "Admin",
                NormalizedUserName = "ADMIN"
            };

            // - - - Set Admin Password - - - 
            PasswordHasher<AppUser> passwordHasher = new PasswordHasher<AppUser>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Mediko2022#");

            // - - - Seed User - - - 
            builder.Entity<AppUser>().HasData(admin);

            // - - - Set AdminRole to user Admin - - -
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleGUID,
                UserId = adminGUID
            });
        }


    }



}

