using System.ComponentModel.DataAnnotations;

namespace MedikoWeb.Models
{
    public class LogViewModel
    {
        public int LogbookId { get; set; }
        public int? logId { get; set; }

        public DateTime DateAndTime { get; set; }
        public float? Value1 { get; set; }
        public float? Value2 { get; set; }
        public float? Value3 { get; set; }
        public string? Comment { get; set; }
    }
}
