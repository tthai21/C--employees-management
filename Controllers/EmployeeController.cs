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

    [HttpGet(Name = "all-employee")]
    public async Task<ActionResult<List<Employee>>> FetchAllEmployees()
    {
        using var _dbContext = new DataContext();
        var employees = (from e in _dbContext.Employees
                         join d in _dbContext.Departments on e.DepartmentId equals d.DepartmentId
                         join r in _dbContext.Roles on e.RoleId equals r.RoleId
                         select new EmployeeDTO
                         {
                             name = e.EmployeeName,
                             email = e.EmployeeEmail,
                             mobile = e.EmployeeMobile,
                             department = d.DepartmentName,
                             role = r.RoleName
                         }).ToList();
        if (employees != null)
        {
            return Ok(employees);
        }
        else
        {
            return NotFound("Failed to fetch employees");
        }

    }
}
