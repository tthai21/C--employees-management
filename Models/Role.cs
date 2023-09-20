using System.ComponentModel.DataAnnotations;

namespace Employee_api
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<Employee> Employees { get; set; }
    }
}