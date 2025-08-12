using Common.Shared.Enums;

namespace Common.Shared.Dtos
{
    public class ReportDto
    {
        public int Id { get; set; }                   // For primary key in DB
        public string Subject { get; set; }           // Başlık
        public ReportPeriod Period { get; set; } = ReportPeriod.Günlük;// Period
        public string Email { get; set; }             // eposta
        public string Directory { get; set; }         // dizin
        public bool Active { get; set; }               // aktif (checkbox)
        public string Query { get; set; }           
        public List<WeekDay> SelectedDays { get; set; } = new();
        public TimeSpan Time { get; set; } = new TimeSpan(9, 0, 0); // default to 09:00
        public DateTime Date { get; set; } = DateTime.Now;
        public string JsonContent { get; set; }       
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string FileType { get; set; } // "Excel", "html", or "her ikisi de"

    }
}
