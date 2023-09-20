using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_api
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeMobile { get; set; }



        [ForeignKey("DepartmentId")]
        public int DepartmentId { get; set; }

        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
    }
}