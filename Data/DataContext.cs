using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace Employee_api
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            DotNetEnv.Env.Load();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }

        public IConfiguration Configuration { get; }

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("DATABASE_CONNECTION_STRING environment variable is not set.");
            }
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public static void InsertData()
        {
            using var dbContext = new DataContext();
            var departments = new[]
        {
            "Development",
            "Quality Assurance",
            "Infrastructure",
            "Technical Support",
            "Security",
            "Project Management",
            "Sales and Marketing"
        };
            foreach (var departmentName in departments)
            {
                if (!dbContext.Departments.Any(d => d.DepartmentName == departmentName))
                {
                    var department = new Department
                    {
                        DepartmentName = departmentName
                    };

                    dbContext.Departments.Add(department);
                }
            }

            dbContext.SaveChanges();

        }
    }

}