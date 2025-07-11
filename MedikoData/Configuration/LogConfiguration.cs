using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedikoData.Entities;

namespace MedikoData.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {

            builder.ToTable("Logs");
            builder.HasOne(l => l.Creator)
                   .WithMany(c => c.Logs).IsRequired();

            builder.HasOne(x => x.LogBook)
                   .WithMany(x => x.Logs)
                   .IsRequired();

        }
    }
}
