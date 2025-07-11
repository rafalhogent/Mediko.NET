
namespace MedikoData.Entities
{
    public class Log
    {
        public int LogId { get; set; }

        public DateTime LogTime { get; set; } = DateTime.Now;
        public float? Value1 { get; set; }
        public float? Value2 { get; set; }
        public float? Value3 { get; set; }
        public string? Comment { get; set; }


        public AppUser Creator { get; set; } = null!;

        public LogBook LogBook { get; set; } = null!;
    }
}
