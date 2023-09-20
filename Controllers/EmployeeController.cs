using Employee_api;
using Microsoft.AspNetCore.Mvc;

namespace C__employees_management.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{

    [HttpPost(Name = "add-employee")]
    public async Task<ActionResult<List<Employee>>> AddEmployee(EmployeeDTO request)
    {
        using var _dbContext = new DataContext();
        var exited = _dbContext.Employees.FirstOrDefault(employee => employee.EmployeeEmail == request.email);
        var department = _dbContext.Departments.FirstOrDefault(d => d.DepartmentName == request.department);
        var role = _dbContext.Roles.FirstOrDefault(d => d.RoleName == request.role);
        if (exited == null)
        {

            var newEmployees = new Employee
            {
                EmployeeName = request.name,
                EmployeeEmail = request.email,
                EmployeeMobile = request.mobile,
                DepartmentId = department.DepartmentId,
                RoleId = role.RoleId,
            };

            _dbContext.Employees.Add(newEmployees);
            _dbContext.SaveChanges();
            return Ok("Added");
        }

        return NotFound("Failed to add employee");
    }

}
