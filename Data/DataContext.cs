using Microsoft.EntityFrameworkCore;

namespace Employee_api
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        private const string connectionString = @"
        Server=localhost\SQLEXPRESS01;
        Database=employeedata;
        Trusted_Connection=True;
        TrustServerCertificate=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }




        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        // }

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