using EmployeeMgmt.Core;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMgmt.Infrastructure.Data;

public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Contractor>();
        modelBuilder.Ignore<Manager>();

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(employee => employee.Id);
            entity.Property(employee => employee.Id).ValueGeneratedOnAdd();
            entity.Property(employee => employee.Name).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.Salary).HasPrecision(18, 2);
            entity.HasOne(employee => employee.Department)
                .WithMany(department => department.Employees)
                .HasForeignKey(employee => employee.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(department => department.Id);
            entity.Property(department => department.Id).ValueGeneratedOnAdd();
            entity.Property(department => department.Name).HasMaxLength(100).IsRequired();
            entity.Navigation(department => department.Employees)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}
