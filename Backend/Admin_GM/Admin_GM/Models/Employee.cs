using System.ComponentModel.DataAnnotations;

namespace Admin_GM.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        
        public string Prename { get; set; }
        
        public string CIN { get; set; }
        
        public string Email { get; set; }
        
        public string Phone_Number { get; set; }
        
        public string Home_Address { get; set; }
        
        public string Emp_Function { get; set; }
        
    }
}
