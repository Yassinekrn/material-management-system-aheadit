namespace Admin_GM.Models
{
    public class Material
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime Purchase_Date { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public int IsTaken { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
