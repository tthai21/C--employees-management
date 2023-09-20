using System.ComponentModel.DataAnnotations;

namespace Employee_api
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public List<Employee> Employees { get; set; }

    }
}