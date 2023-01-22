namespace Admin_GM.Models
{
    public class HRRecord
    {
        public int ID { get; set; }
        public int ID_Employee { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;

        public Employee? Employee { get; set; }
    }
}
