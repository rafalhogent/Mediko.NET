using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedikoData.Entities;

namespace MedikoData.Configurations
{
    public class LogBookConfiguration : IEntityTypeConfiguration<LogBook>
    {
        public void Configure(EntityTypeBuilder<LogBook> builder)
        {

            //* - - - - - Seeding - - - - - - -
            builder.HasData(
                new LogBook { LogBookId = 1, Name = "Dagboek" },
                new LogBook { LogBookId = 2, Name = "Gewicht", Field1 = "gewicht", Unit1 = "kg", Precision = 1 },
                new LogBook
                {
                    LogBookId = 3,
                    Name = "Bloeddruk",
                    Field1 = "systolic",
                    Field2 = "diastolic",
                    Field3 = "pulse",
                    Precision = 0
                },

                new LogBook { LogBookId = 4, Name = "Temperatuur", Field1 = "temperatuur", Unit1 = "°C", Precision = 1 },
                new LogBook { LogBookId = 5, Name = "Water", Field1 = "drink", Unit1 = "ml", Precision = 1 },
                new LogBook { LogBookId = 6, Name = "Glucose", Field1 = "glucose", Unit1 = "mmol/L", Precision = 1 }
                );
            // - - - - - - - - - - - - - - - - - */
        }
    }
}
