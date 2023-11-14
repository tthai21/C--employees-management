using Employee_api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace C__employees_management.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    [HttpPost("add-employee")]
    public async Task<ActionResult<List<Employee>>> AddEmployee(EmployeeDTO request)
    {
        using var _dbContext = new DataContext();
        var exited = await _dbContext.Employees.FirstOrDefaultAsync(employee => employee.EmployeeEmail == request.email);
        var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.DepartmentName == request.department);
        var role = await _dbContext.Roles.FirstOrDefaultAsync(d => d.RoleName == request.role);
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
            return Ok($"Employee {newEmployees.EmployeeName} had been added to the database");
        }

        return NotFound("Failed to add employee");
    }

    [HttpGet("all-employees")]
    public async Task<ActionResult<List<EmployeeDTO>>> AllEmployees()
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

    [HttpGet("sort-employees")]
    public ActionResult<EmployeeDTO> SortEmployees(string department = null, string role = null)
    {
        using var db_Context = new DataContext();
        var employees = from e in db_Context.Employees
                        join d in db_Context.Departments on e.DepartmentId equals d.DepartmentId
                        join r in db_Context.Roles on e.RoleId equals r.RoleId
                        select new EmployeeDTO
                        {
                            id = e.DepartmentId,
                            name = e.EmployeeName,
                            email = e.EmployeeEmail,
                            mobile = e.EmployeeMobile,
                            department = d.DepartmentName,
                            role = r.RoleName
                        };
        if (!string.IsNullOrEmpty(department))
        {
            employees = employees.Where(e => e.department == department);
        }
        if (!string.IsNullOrEmpty(role))
        {
            employees = employees.Where(e => e.role == role);
        }

        var sortedEmployees = employees.ToList();

        if (sortedEmployees != null && sortedEmployees.Count > 0)
        {
            return Ok(sortedEmployees);
        }
        else
        {
            return NotFound("No employees found with the specified criteria.");
        }
    }


    [HttpPost("edit/employeeId")]
    public async Task<ActionResult<EmployeeDTO>> EditEmployee(EmployeeDTO request)
    {
        using var _dbContext = new DataContext();
        var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.DepartmentName == request.department);
        var role = await _dbContext.Roles.FirstOrDefaultAsync(d => d.RoleName == request.role);
        var currentEmployee = _dbContext.Employees.Where(employee => employee.EmployeeId == request.id).FirstOrDefault();
        if (currentEmployee != null)
        {
            currentEmployee.EmployeeName = request.name;
            currentEmployee.EmployeeEmail = request.email;
            currentEmployee.EmployeeMobile = request.mobile;
            currentEmployee.DepartmentId = department.DepartmentId;
            currentEmployee.RoleId = role.RoleId;
            _dbContext.SaveChanges();
            return Ok($"Employee's information has been update successfully to.");
        }
        else
        {
            return NotFound("Employee not found");
        }
    }

    [HttpPost("remove/employeeId")]
    public async Task<ActionResult> RemoveEmployee([FromQuery] int id)
    {
        using var _dbContext = new DataContext();


        var currentEmployee = await _dbContext.Employees.FindAsync(id);
        if (currentEmployee != null)
        {
            _dbContext.Employees.Remove(currentEmployee);
            await _dbContext.SaveChangesAsync();
            return Ok($"Employee with ID {id} has been successfully removed.");
        }
        else
        {
            return NotFound("Employee not found");
        }
    }

}
