using Microsoft.AspNetCore.Identity;

namespace MedikoData.Entities
{
    public enum Gender
    {
        Man, Woman, X
    }

    public enum Language
    {
        EN, NL, FR, PL, DE, ES
    }
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Language Language { get; set; } = Language.NL;


        public ICollection<LogBook> CustomLogbooks { get; set; } = null!;
        public ICollection<Log> Logs { get; set; } = null!;

        public ICollection<LogBook> ChoosenLogbooks { get; set; } = null!;
    }
}
