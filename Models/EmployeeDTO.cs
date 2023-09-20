using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_api
{
    public class EmployeeDTO
    {
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string department { get; set; }
        public string role { get; set; }
    }
}