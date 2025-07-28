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

        public string Query { get; set; }             // Optional, not in grid
        public List<string> SelectedDays { get; set; } = new(); // Optional, not in grid

        public string JsonContent { get; set; }       // Stored in DB, not used directly in UI
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
