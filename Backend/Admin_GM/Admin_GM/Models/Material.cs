namespace Admin_GM.Models
{
    public class Material
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime Purchase_Date { get; set; }
        public string Brand { get; set; }
        public string Reference { get; set; }
        public int IsTaken { get; set; }
    }
}
