
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedikoData.Entities;

namespace MedikoData.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<AppUser>
    {


        public void Configure(EntityTypeBuilder<AppUser> builder)
        {

            builder.Property(u => u.FirstName).HasMaxLength(60);
            builder.Property(u => u.LastName).HasMaxLength(80);

            builder.HasMany(x => x.CustomLogbooks).WithOne(x => x.Creator);
            builder.HasMany(x => x.Logs).WithOne(x => x.Creator);

            builder.HasMany(x => x.ChoosenLogbooks).WithMany(x => x.UsersWhoChoosen);

        }

    }

}
