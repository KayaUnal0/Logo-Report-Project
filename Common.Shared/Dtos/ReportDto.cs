using Common.Shared.Enums;

namespace Common.Shared.Dtos
{
    public class ReportDto
    {
        public int Id { get; set; }                   // For primary key in DB
        public string Subject { get; set; }           // Başlık
        public string Period { get; set; }            // Period
        public string Email { get; set; }             // eposta
        public string Directory { get; set; }         // dizin
        public bool Aktif { get; set; }               // aktif (checkbox)

        public string Query { get; set; }           
        public List<WeekDay> SelectedDays { get; set; } = new(); 

        public string JsonContent { get; set; }       
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
