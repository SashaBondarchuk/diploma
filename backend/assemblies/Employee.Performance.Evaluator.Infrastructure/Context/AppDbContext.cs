using System.Globalization;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context.Extensions;
using Microsoft.EntityFrameworkCore;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; private set; }
    public DbSet<EmployeeEntity> Employees { get; private set; }
    public DbSet<Team> Teams { get; private set; }
    public DbSet<Role> Roles { get; private set; }
    public DbSet<Permission> Permissions { get; private set; }
    public DbSet<RolePermission> RolePermissions { get; private set; }
    public DbSet<KPIMetric> KPIMetrics { get; private set; }
    public DbSet<RoleKPI> RoleKPIs { get; private set; }
    public DbSet<Evaluation> Evaluations { get; private set; }
    public DbSet<EvaluationSession> EvaluationSessions { get; private set; }
    public DbSet<EmployeeClass> EmployeeClasses { get; private set; }
    public DbSet<Recommendation> Recommendations { get; private set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Configure();

        modelBuilder.Seed();
    }
}