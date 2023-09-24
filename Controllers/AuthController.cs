using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Employee_api;
using System.Text;

namespace C__employees_management.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public static User user = new User();

    private IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDTO request)
    {
        using var _dbContext = new DataContext();

        bool userExists = await _dbContext.Users.AnyAsync(user => user.UserEmail == request.email);
        if (userExists)
        {
            var errorResponse = new { ErrorMessage = "Email already exists" };
            return Conflict(errorResponse);
        }


        var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.DepartmentName == request.department);
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == request.role);

        if (department == null || role == null)
        {
            var errorResponse = new { ErrorMessage = "Department or role not found" };
            return NotFound(errorResponse);
        }


        CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);


        var newUser = new User
        {
            UserName = request.name,
            UserEmail = request.email,
            UserMobile = request.mobile,
            DepartmentId = department.DepartmentId,
            RoleId = role.RoleId,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return Ok(newUser);
    }


    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(UserDTO request)
    {
        using var _dbContext = new DataContext();
        user = _dbContext.Users.FirstOrDefault(user => user.UserEmail == request.email);

        if (user == null)
        {
            var errorResponse = new { ErrorMessage = "Email not found" };
            return NotFound(errorResponse);
        }
        if (!VerifyPasswordHash(request.password, user.PasswordHash, user.PasswordSalt))
        {
            var errorResponse = new { ErrorMessage = "Incorrect Password" };
            return NotFound(errorResponse);
        }

        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == user.RoleId);
        string token = CreateToken(user, role.RoleName);

        return Ok(token);

    }





    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user, string role)
    {


        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.UserEmail!),
            new Claim(ClaimTypes.Role, role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:securityKey")));
        var tokenHandler = new JwtSecurityTokenHandler();
        var cred = new SigningCredentials(key!, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = cred
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}