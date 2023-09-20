using Employee_api;
using Microsoft.AspNetCore.Mvc;

namespace C__employees_management.Controllers;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase
{

    [HttpPost("add-employee")]
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

    [HttpGet("all-employee")]
    public async Task<ActionResult<List<EmployeeDTO>>> FetchAllEmployees()
    {
        using var _dbContext = new DataContext();
        var employees = (from e in _dbContext.Employees
                         join d in _dbContext.Departments on e.DepartmentId equals d.DepartmentId
                         join r in _dbContext.Roles on e.RoleId equals r.RoleId
                         select new EmployeeDTO
                         {
                             id = e.EmployeeId,
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

    [HttpPost("employeeId")]
    public async Task<ActionResult<EmployeeDTO>> UpdateOneEmployee(EmployeeDTO request)
    {
        using var _dbContext = new DataContext();
        var department = _dbContext.Departments.FirstOrDefault(d => d.DepartmentName == request.department);
        var role = _dbContext.Roles.FirstOrDefault(d => d.RoleName == request.role);
        var currentEmployee = _dbContext.Employees.Where(employee => employee.EmployeeId == request.id).FirstOrDefault();
        if (currentEmployee != null)
        {
            currentEmployee.EmployeeName = request.name;
            currentEmployee.EmployeeEmail = request.email;
            currentEmployee.EmployeeMobile = request.mobile;
            currentEmployee.DepartmentId = department.DepartmentId;
            currentEmployee.RoleId = role.RoleId;
            _dbContext.SaveChanges();
            return Ok(currentEmployee);
        }
        else
        {
            return NotFound("Employee not found");
        }

    }
}
