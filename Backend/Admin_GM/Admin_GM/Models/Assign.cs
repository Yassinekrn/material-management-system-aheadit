namespace Admin_GM.Models
{
    public class Assign
    {
        public int ID { get; set; }
        public int ID_Employee { get; set; }
        public int ID_Material { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
    }
}
