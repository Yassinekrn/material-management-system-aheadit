using System.ComponentModel.DataAnnotations;

namespace Admin_GM.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Prename { get; set; } = string.Empty;
        public string CIN { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone_Number { get; set; } = string.Empty;
        public string Home_Address { get; set; } = string.Empty;
        public string Emp_Function { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
