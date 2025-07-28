namespace Common.Shared.Dtos
{
    public class ReportDto
    {
        public string Subject { get; set; }
        public string Email { get; set; }
        public string Query { get; set; }
        public string Directory { get; set; } 
        public string Period { get; set; }
        public List<string> SelectedDays { get; set; } = new();

    }

}
