using Bogus;
using Employee.Performance.Evaluator.Core.Entities;
using Microsoft.EntityFrameworkCore;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Infrastructure.Context.Extensions;

public static class ModelBuilderExtensions
{
    public static void Configure(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Role)
                  .WithMany()
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeeEntity>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Avatar).HasColumnType("varbinary(max)");

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasOne(e => e.User)
                  .WithOne()
                  .HasForeignKey<EmployeeEntity>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Team)
                  .WithMany(e => e.Employees)
                  .HasForeignKey(e => e.TeamId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Teams");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.TeamLead)
                  .WithMany()
                  .HasForeignKey(e => e.TeamLeadId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.RoleName).IsUnique();

            entity.HasMany(r => r.Permissions)
                  .WithMany()
                  .UsingEntity<RolePermission>();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<KPIMetric>(entity =>
        {
            entity.ToTable("KPIMetrics");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<RoleKPI>(entity =>
        {
            entity.ToTable("RoleKPIs");
            entity.HasKey(e => new { e.RoleId, e.KpiId });
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)").IsRequired();

            entity.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.KpiMetric).WithMany().HasForeignKey(e => e.KpiId);
        });

        modelBuilder.Entity<EvaluationSession>(entity =>
        {
            entity.ToTable("EvaluationSessions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ReportFile).HasColumnType("varbinary(max)");
            entity.Property(e => e.WeightedScore).HasColumnType("decimal(5, 2)");

            entity.HasOne(es => es.Employee)
                  .WithMany(e => e.EvaluationSessions)
                  .HasForeignKey(es => es.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(es => es.Class)
                  .WithMany()
                  .HasForeignKey(es => es.ClassId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.ToTable("Evaluations");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.EvaluationSession)
                  .WithMany(es => es.Evaluations)
                  .HasForeignKey(e => e.EvaluationSessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Evaluator)
                  .WithMany()
                  .HasForeignKey(e => e.EvaluatorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RoleKpi)
                  .WithMany()
                  .HasForeignKey(e => new { e.RoleId, e.KpiId })
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeeClass>(entity =>
        {
            entity.ToTable("EmployeeClasses");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ClassName).IsUnique();
            entity.Property(e => e.ClassName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.MinScore).HasColumnType("decimal(5, 2)").IsRequired();
            entity.Property(e => e.MaxScore).HasColumnType("decimal(5, 2)").IsRequired();
        });

        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.ToTable("Recommendations");
            entity.HasKey(e => e.Id);

            entity.HasOne<EmployeeEntity>()
                  .WithMany(e => e.Recommendations)
                  .HasForeignKey(r => r.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public static void Seed(this ModelBuilder modelBuilder)
    {
        var seed = 8675309;
        var now = new DateTime(2025, 01, 01);

        var roles = new[]
        {
            new Role { Id = 1, RoleName = "Unassigned" },
            new Role { Id = 2, RoleName = "Manager" },
            new Role { Id = 3, RoleName = "Developer" },
            new Role { Id = 4, RoleName = "QA Engineer" },
            new Role { Id = 5, RoleName = "HR" },
            new Role { Id = 6, RoleName = "Team Lead Developer" },
        };

        modelBuilder.Entity<Role>().HasData(roles);

        var permissions = new[]
        {
            new Permission { Id = 1, Name = "base", Description = "View employees; View teams; View own recommendations; View own evaluations & reports;" },

            new Permission { Id = 2, Name = "manage_roles", Description = "Manage roles and assign permissions" },
            new Permission { Id = 3, Name = "manage_kpis", Description = "Manage KPI metrics and assign to roles" },
            new Permission { Id = 4, Name = "manage_employees", Description = "Manage employee profiles" },
            new Permission { Id = 5, Name = "manage_teams", Description = "Manage teams" },
            new Permission { Id = 6, Name = "manage_evaluations", Description = "Manage evaluation sessions" },
            new Permission { Id = 7, Name = "create_recommendations", Description = "Create recommendations based on evaluation sessions" },
            new Permission { Id = 8, Name = "view_all_evaluations", Description = "View all evaluations" },

            new Permission { Id = 9, Name = "view_team_evaluations", Description = "View evaluations of employees from the team" },

            new Permission { Id = 10, Name = "evaluate_team_members", Description = "Evaluate employees from the team based on role metrics" },
        };

        modelBuilder.Entity<Permission>().HasData(permissions);

        var rolePermissions = new[]
        {
            // Manager
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 3 },
            new RolePermission { RoleId = 2, PermissionId = 4 },
            new RolePermission { RoleId = 2, PermissionId = 5 },
            new RolePermission { RoleId = 2, PermissionId = 6 },
            new RolePermission { RoleId = 2, PermissionId = 7 },
            new RolePermission { RoleId = 2, PermissionId = 8 },

            // Team Lead Developer
            new RolePermission { RoleId = 6, PermissionId = 1 },
            new RolePermission { RoleId = 6, PermissionId = 9 },
            new RolePermission { RoleId = 6, PermissionId = 10 },

            // Developer
            new RolePermission { RoleId = 3, PermissionId = 1 },
            new RolePermission { RoleId = 3, PermissionId = 10 },
        };

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

        var kpiMetrics = new[]
        {
            new KPIMetric { Id = 1, Name = "Code Quality" },
            new KPIMetric { Id = 2, Name = "Development Speed" },
            new KPIMetric { Id = 3, Name = "Team Collaboration" },
            new KPIMetric { Id = 4, Name = "Bug Detection" },
            new KPIMetric { Id = 5, Name = "Documentation Quality" },
            new KPIMetric { Id = 6, Name = "Process Improvement" },
            new KPIMetric { Id = 7, Name = "Time Management" },
            new KPIMetric { Id = 8, Name = "Initiative" }
        };

        modelBuilder.Entity<KPIMetric>().HasData(kpiMetrics);

        var roleKPIs = new Faker<RoleKPI>()
            .UseSeed(seed)
            .RuleFor(rk => rk.RoleId, f => f.PickRandom(roles).Id)
            .RuleFor(rk => rk.KpiId, f => f.PickRandom(kpiMetrics).Id)
            .RuleFor(rk => rk.Weight, f => f.Random.Decimal(5, 20))
            .RuleFor(rk => rk.IsAllowedToEvaluateExceptLead, f => f.Random.Bool())
            .RuleFor(rk => rk.MinScore, f => 1)
            .RuleFor(rk => rk.MaxScore, f => 10)
            .RuleFor(rk => rk.ScoreRangeDescription, f => f.Lorem.Sentence())
            .Generate(10)
            .DistinctBy(rk => new { rk.RoleId, rk.KpiId })
            .Take(10)
            .ToList();

        modelBuilder.Entity<RoleKPI>().HasData(roleKPIs);

        var employeeClasses = new[]
        {
            new EmployeeClass
            {
                Id = 1,
                ClassName = "A",
                MinScore = 8.50m,
                MaxScore = 10.00m,
                Description = "High level of professionalism, consistently high results, positive impact on the team and processes.",
                RecommendedActions = new[]
                {
                    "- Involve in mentoring newcomers",
                    "- Assign more complex or strategic tasks",
                    "- Suggest development towards technical leadership or architecture"
                }
            },
            new EmployeeClass
            {
                Id = 2,
                ClassName = "B",
                MinScore = 7.00m,
                MaxScore = 8.49m,
                Description = "Confidently performs tasks, demonstrates good results, may require minor adjustments in certain aspects.",
                RecommendedActions = new[]
                {
                    "- Maintain the current level",
                    "- Identify 1-2 areas for development (soft skills or technical competencies)",
                    "- Encourage participation in team processes (code review, technical discussions)"
                }
            },
            new EmployeeClass
            {
                Id = 3,
                ClassName = "C",
                MinScore = 5.50m,
                MaxScore = 6.99m,
                Description = "Generally copes with the work, but there are noticeable areas for improvement. Requires attention from the manager or mentor.",
                RecommendedActions = new[]
                {
                    "- Regular feedback with clear examples",
                    "- Recommend internal training",
                    "- Involve in pair programming or group work to increase efficiency"
                }
            },
            new EmployeeClass
            {
                Id = 4,
                ClassName = "D",
                MinScore = 4.00m,
                MaxScore = 5.49m,
                Description = "Significant difficulties in performing tasks, often requires help, insufficient quality or speed of work.",
                RecommendedActions = new[]
                {
                    "- Develop a development plan with specific goals and deadlines",
                    "- Assign a mentor",
                    "- Control the quality of completed work with regular feedback"
                }
            },
            new EmployeeClass
            {
                Id = 5,
                ClassName = "E",
                MinScore = 1.00m,
                MaxScore = 3.99m,
                Description = "Work does not meet expectations, serious problems with quality, communication, or professional skills.",
                RecommendedActions = new[]
                {
                    "- Immediate conversation with the manager about the future in the company",
                    "- Creating an individual improvement plan",
                    "- Control results within 1-3 months with recording of progress or regression"
                }
            }
        };

        modelBuilder.Entity<EmployeeClass>().HasData(employeeClasses);

        var users = new Faker<User>()
            .UseSeed(seed)
            .RuleFor(u => u.Id, f => f.IndexFaker + 1)
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.RoleId, f => f.PickRandom(roles).Id)
            .Generate(20);

        modelBuilder.Entity<User>().HasData(users);

        var teams = new Faker<Team>()
            .UseSeed(seed)
            .RuleFor(t => t.Id, f => f.IndexFaker + 1)
            .RuleFor(t => t.Name, f => f.Commerce.Department())
            .RuleFor(t => t.TeamLeadId, _ => null)
            .Generate(10);

        modelBuilder.Entity<Team>().HasData(teams);

        var employees = new Faker<EmployeeEntity>()
            .UseSeed(seed)
            .RuleFor(e => e.Id, f => f.IndexFaker + 1)
            .RuleFor(e => e.UserId, (f, e) => users[e.Id - 1].Id)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(e => e.BirthDate, f => f.Date.Past(40, now.AddYears(-18)))
            .RuleFor(e => e.HireDate, f => f.Date.Past(5, now))
            .RuleFor(e => e.TeamId, f => f.PickRandom(teams).Id)
            .RuleFor(e => e.Avatar, f => f.Image.Random.Bytes(1000))
            .Generate(20);

        modelBuilder.Entity<EmployeeEntity>().HasData(employees);

        var evaluationSessions = new Faker<EvaluationSession>()
            .UseSeed(seed)
            .RuleFor(es => es.Id, f => f.IndexFaker + 1)
            .RuleFor(es => es.EmployeeId, f => f.PickRandom(employees).Id)
            .RuleFor(es => es.ClassId, f => f.PickRandom(employeeClasses).Id)
            .RuleFor(es => es.StartDate, f => f.Date.Past(3, now))
            .RuleFor(es => es.EndDate, f => f.Date.Past(1, now))
            .RuleFor(es => es.EvaluationFinishedDate, f => f.Date.Past(2, now))
            .Generate(10);

        modelBuilder.Entity<EvaluationSession>().HasData(evaluationSessions);

        var evaluations = new List<Evaluation>();
        int evaluationId = 1;
        foreach (var session in evaluationSessions)
        {
            var numMetrics = new Random(seed).Next(2, 5);
            var randomOrderSelector = new Random(seed).Next(0, 2) == 0 ? "Weight" : "KpiId";
            var selectedRoleKPIs = roleKPIs.OrderBy(x => randomOrderSelector).Take(numMetrics).ToList();
            foreach (var roleKPI in selectedRoleKPIs)
            {
                evaluations.Add(new Evaluation
                {
                    Id = evaluationId++,
                    EvaluationSessionId = session.Id,
                    EvaluatorId = employees[new Random(seed).Next(1, employees.Count)].Id,
                    KpiId = roleKPI.KpiId,
                    RoleId = roleKPI.RoleId,
                    Score = new Random(seed).Next(1, 6),
                    Comment = "Some comment lorem ipsum dolor amet",
                });
            }
        }

        modelBuilder.Entity<Evaluation>().HasData(evaluations);

        var recommendations = new Faker<Recommendation>()
            .UseSeed(seed)
            .RuleFor(r => r.Id, f => f.IndexFaker + 1)
            .RuleFor(r => r.EmployeeId, f => f.PickRandom(employees).Id)
            .RuleFor(r => r.RecommendationText, f => f.Lorem.Paragraph())
            .RuleFor(r => r.CreatedAt, f => f.Date.Past(1, now))
            .Generate(10);

        modelBuilder.Entity<Recommendation>().HasData(recommendations);
    }
}
