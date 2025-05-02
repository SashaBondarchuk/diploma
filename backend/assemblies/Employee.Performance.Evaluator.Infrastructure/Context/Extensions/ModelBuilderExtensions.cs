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
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 4)").IsRequired();

            entity.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.KpiMetric).WithMany().HasForeignKey(e => e.KpiId);
        });

        modelBuilder.Entity<EvaluationSession>(entity =>
        {
            entity.ToTable("EvaluationSessions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ReportFile).HasColumnType("varbinary(max)");
            entity.Property(e => e.WeightedScore).HasColumnType("decimal(5, 4)");

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
                  .WithMany()
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

            entity.HasOne(e => e.Employee)
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
            new Role { Id = 7, RoleName = "Intern Developer" },
            new Role { Id = 8, RoleName = "Scrum Master" },
            new Role { Id = 9, RoleName = "Product Owner" }
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

            new Permission { Id = 9, Name = "evaluate_team_members_lead", Description = "Evaluate employees from the team based on all role metrics" },

            new Permission { Id = 10, Name = "evaluate_team_members", Description = "Evaluate employees from the team based on specific role metrics" },
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
            new RolePermission { RoleId = 2, PermissionId = 9 },
            new RolePermission { RoleId = 2, PermissionId = 10 },

            // Developer
            new RolePermission { RoleId = 3, PermissionId = 1 },
            new RolePermission { RoleId = 3, PermissionId = 10 },

            // QA Engineer
            new RolePermission { RoleId = 4, PermissionId = 1 },
            new RolePermission { RoleId = 4, PermissionId = 10 },

            // HR
            new RolePermission { RoleId = 5, PermissionId = 1 },
            new RolePermission { RoleId = 5, PermissionId = 4 },
            new RolePermission { RoleId = 5, PermissionId = 5 },
            // new RolePermission { RoleId = 5, PermissionId = 8 },
            new RolePermission { RoleId = 5, PermissionId = 10 },

            // Team Lead Developer
            new RolePermission { RoleId = 6, PermissionId = 1 },
            new RolePermission { RoleId = 6, PermissionId = 9 },
            new RolePermission { RoleId = 6, PermissionId = 10 },

            // Intern
            new RolePermission { RoleId = 7, PermissionId = 1 },

            // Scrum Master
            new RolePermission { RoleId = 8, PermissionId = 1 },
            new RolePermission { RoleId = 8, PermissionId = 10 },

            // Product Owner
            new RolePermission { RoleId = 9, PermissionId = 1 },
            new RolePermission { RoleId = 9, PermissionId = 10 },
        };

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

        var kpiMetrics = new[]
        {
            // Developer
            new KPIMetric { Id = 1, Name = "Code Quality" },
            new KPIMetric { Id = 2, Name = "Development Speed" },
            new KPIMetric { Id = 3, Name = "Team Support" },
            new KPIMetric { Id = 4, Name = "Deadline Compliance" },
            new KPIMetric { Id = 5, Name = "Estimation Accuracy" },
            new KPIMetric { Id = 6, Name = "Improvement Initiatives" },

            // Intern
            new KPIMetric { Id = 7, Name = "Learning Speed" },
            new KPIMetric { Id = 8, Name = "Task Quality (Intern)" },
            new KPIMetric { Id = 9, Name = "Initiative (Intern)" },
            new KPIMetric { Id = 10, Name = "Internal Participation" },
            new KPIMetric { Id = 11, Name = "Teamwork (Intern)" },
            new KPIMetric { Id = 12, Name = "Feedback Response" },
            new KPIMetric { Id = 13, Name = "Deadline Adherence (Intern)" },
            new KPIMetric { Id = 14, Name = "Independence" },

            // QA Engineer
            new KPIMetric { Id = 15, Name = "Bugs Found" },
            new KPIMetric { Id = 16, Name = "Testing Speed" },
            new KPIMetric { Id = 17, Name = "Test Documentation Quality" },
            new KPIMetric { Id = 18, Name = "Testing Process Improvements" },

            // Team Lead
            new KPIMetric { Id = 19, Name = "Team Productivity" },
            new KPIMetric { Id = 20, Name = "Code Review Quality" },
            new KPIMetric { Id = 21, Name = "Team Development Efforts" },
            new KPIMetric { Id = 22, Name = "Communication Quality" },
            new KPIMetric { Id = 23, Name = "Release Timeliness" },
            new KPIMetric { Id = 24, Name = "Conflict Resolution" },
            new KPIMetric { Id = 25, Name = "Cross-Team Coordination" },

            // Scrum Master
            new KPIMetric { Id = 26, Name = "Meeting Effectiveness" },
            new KPIMetric { Id = 27, Name = "Agile Process Compliance" },
            new KPIMetric { Id = 28, Name = "Blocker Resolution" },
            new KPIMetric { Id = 29, Name = "Team Engagement" },
            new KPIMetric { Id = 30, Name = "Development Cycle Time" },
            new KPIMetric { Id = 31, Name = "Process Enhancements" },
            new KPIMetric { Id = 32, Name = "Team Training Activities" },
            new KPIMetric { Id = 33, Name = "Team Satisfaction" },

            // Product Owner
            new KPIMetric { Id = 34, Name = "Backlog Quality" },
            new KPIMetric { Id = 35, Name = "Priority Alignment" },
            new KPIMetric { Id = 36, Name = "Backlog Maintenance" },
            new KPIMetric { Id = 37, Name = "User Satisfaction" },

            // HR
            new KPIMetric { Id = 38, Name = "Hiring Speed" },
            new KPIMetric { Id = 39, Name = "Hiring Quality" },
            new KPIMetric { Id = 40, Name = "Internal Events" },
            new KPIMetric { Id = 41, Name = "Employee Retention" },
            new KPIMetric { Id = 42, Name = "Onboarding Effectiveness" },
            new KPIMetric { Id = 43, Name = "Culture Development" }
        };

        modelBuilder.Entity<KPIMetric>().HasData(kpiMetrics);

        var kpiDescriptions = kpiMetrics.ToDictionary(
            kpi => kpi.Id,
            kpi => kpi.Name switch
            {
                "Code Quality" => "Score 1–100: 1 = Poor, buggy code; 100 = Clean, maintainable code.",
                "Development Speed" => "Score 1–100: 1 = Slow, frequent delays; 100 = Delivers very fast.",
                "Team Support" => "Score 1–100: 1 = Rarely helps team; 100 = Always supportive and collaborative.",
                "Deadline Compliance" => "Score 1–100: 1 = Often misses deadlines; 100 = Always meets them.",
                "Estimation Accuracy" => "Score 1–100: 1 = Highly inaccurate estimates; 100 = Very precise estimates.",
                "Improvement Initiatives" => "Score 1–100: 1 = No initiatives; 100 = Regular, high-impact improvements.",
                "Learning Speed" => "Score 1–100: 1 = Learns very slowly; 100 = Adapts and learns extremely quickly.",
                "Task Quality (Intern)" => "Score 1–100: 1 = Incomplete or low-quality tasks; 100 = High-quality output.",
                "Initiative (Intern)" => "Score 1–100: 1 = Passive, needs direction; 100 = Highly proactive.",
                "Internal Participation" => "Score 1–100: 1 = Rarely participates; 100 = Very engaged in team events.",
                "Teamwork (Intern)" => "Score 1–100: 1 = Poor collaborator; 100 = Excellent team player.",
                "Feedback Response" => "Score 1–100: 1 = Ignores feedback; 100 = Always improves based on feedback.",
                "Deadline Adherence (Intern)" => "Score 1–100: 1 = Often late; 100 = Always on time.",
                "Independence" => "Score 1–100: 1 = Needs constant help; 100 = Fully independent.",
                "Bugs Found" => "Score 1–100: 1 = Misses obvious bugs; 100 = Detects most critical bugs.",
                "Testing Speed" => "Score 1–100: 1 = Very slow tester; 100 = Tests thoroughly and quickly.",
                "Test Documentation Quality" => "Score 1–100: 1 = Incomplete/confusing test cases; 100 = Clear and detailed.",
                "Testing Process Improvements" => "Score 1–100: 1 = No contributions; 100 = Major positive changes to QA process.",
                "Team Productivity" => "Score 1–100: 1 = Team underperforms; 100 = Highly productive team.",
                "Code Review Quality" => "Score 1–100: 1 = Unhelpful reviews; 100 = Deep, constructive feedback.",
                "Team Development Efforts" => "Score 1–100: 1 = No mentoring; 100 = Actively helps others grow.",
                "Communication Quality" => "Score 1–100: 1 = Unclear communication; 100 = Very clear and effective.",
                "Release Timeliness" => "Score 1–100: 1 = Always late; 100 = Always on schedule.",
                "Conflict Resolution" => "Score 1–100: 1 = Escalates conflicts; 100 = Resolves calmly and effectively.",
                "Cross-Team Coordination" => "Score 1–100: 1 = Poor coordination; 100 = Excellent collaboration across teams.",
                "Meeting Effectiveness" => "Score 1–100: 1 = Unproductive meetings; 100 = Highly effective meetings.",
                "Agile Process Compliance" => "Score 1–100: 1 = Rarely follows process; 100 = Strictly adheres to Agile.",
                "Blocker Resolution" => "Score 1–100: 1 = Slow to unblock team; 100 = Quickly removes blockers.",
                "Team Engagement" => "Score 1–100: 1 = Team is disengaged; 100 = Highly engaged team.",
                "Development Cycle Time" => "Score 1–100: 1 = Long delays; 100 = Rapid, smooth cycle.",
                "Process Enhancements" => "Score 1–100: 1 = No suggestions; 100 = Regular, valuable process improvements.",
                "Team Training Activities" => "Score 1–100: 1 = No training; 100 = Frequent, helpful training sessions.",
                "Team Satisfaction" => "Score 1–100: 1 = Low morale; 100 = Very satisfied and motivated team.",
                "Backlog Quality" => "Score 1–100: 1 = Confusing, unclear items; 100 = Well-defined, actionable items.",
                "Priority Alignment" => "Score 1–100: 1 = Misaligned goals; 100 = Perfect alignment with business priorities.",
                "Backlog Maintenance" => "Score 1–100: 1 = Neglected backlog; 100 = Always up-to-date and prioritized.",
                "User Satisfaction" => "Score 1–100: 1 = Many complaints; 100 = Excellent user feedback.",
                "Hiring Speed" => "Score 1–100: 1 = Very slow hiring; 100 = Extremely fast and efficient hiring.",
                "Hiring Quality" => "Score 1–100: 1 = Poor hires; 100 = High-quality, long-term hires.",
                "Internal Events" => "Score 1–100: 1 = No contribution; 100 = Organizes engaging events regularly.",
                "Employee Retention" => "Score 1–100: 1 = High turnover; 100 = Excellent retention.",
                "Onboarding Effectiveness" => "Score 1–100: 1 = Disorganized; 100 = Smooth and thorough onboarding.",
                "Culture Development" => "Score 1–100: 1 = No culture impact; 100 = Drives strong company culture.",
                _ => "Score 1–100: 1 = Poor performance; 100 = Excellent performance."
            }
        );

        var roleToKpis = new Dictionary<int, List<int>>
        {
            [3] = new() { 1, 2, 3, 4, 5, 6 }, // Developer
            [7] = new() { 7, 8, 9, 10, 11, 12, 13, 14 }, // Intern
            [4] = new() { 15, 16, 17, 18 }, // QA Engineer
            [6] = new() { 19, 20, 21, 22, 23, 24, 25 }, // Team Lead Developer
            [8] = new() { 26, 27, 28, 29, 30, 31, 32, 33 }, // Scrum Master
            [9] = new() { 34, 35, 36, 37 }, // Product Owner
            [5] = new() { 38, 39, 40, 41, 42, 43 } // HR
        };

        var roleKPIs = new List<RoleKPI>();
        foreach (var role in roles)
        {
            var kpiIds = kpiMetrics.OrderBy(k => k.Id).Select(k => k.Id).Take(5).ToList();
            var random = new Random(role.Id);

            var rawWeights = kpiIds.Select(_ => (decimal)random.Next(1, 10000)).ToList();
            var sum = rawWeights.Sum();

            decimal runningTotal = 0;

            for (int i = 0; i < kpiIds.Count; i++)
            {
                var kpiId = kpiIds[i];
                decimal weight;

                if (i < kpiIds.Count - 1)
                {
                    weight = Math.Round(rawWeights[i] / sum, 4);
                    runningTotal += weight;
                }
                else
                {
                    // compensate rounding difference on last item
                    weight = Math.Round(1.0000m - runningTotal, 4);
                }

                roleKPIs.Add(new RoleKPI
                {
                    RoleId = role.Id,
                    KpiId = kpiId,
                    Weight = weight,
                    MinScore = 1,
                    MaxScore = 100,
                    IsAllowedToEvaluateExceptLead = true,
                    ScoreRangeDescription = kpiDescriptions.TryGetValue(kpiId, out var desc)
                        ? desc
                        : "Score 1–100: 1 = Poor, 100 = Excellent"
                });
            }
        }

        modelBuilder.Entity<RoleKPI>().HasData(roleKPIs);

        var employeeClasses = new[]
        {
            new EmployeeClass
            {
                Id = 1,
                ClassName = "A",
                MinScore = 85.0m,
                MaxScore = 100m,
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
                MinScore = 70.0m,
                MaxScore = 84.99m,
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
                MinScore = 55.0m,
                MaxScore = 69.99m,
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
                MinScore = 40.0m,
                MaxScore = 54.99m,
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
                MinScore = 0.0m,
                MaxScore = 39.99m,
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
            .RuleFor(es => es.Name, f => f.Lorem.Sentence())
            .RuleFor(es => es.EmployeeId, f => f.PickRandom(employees).Id)
            .RuleFor(es => es.ClassId, f => null)
            .RuleFor(es => es.StartDate, f => f.Date.Past(3, now))
            .RuleFor(es => es.EndDate, f => f.Date.Future(60, now))
            .RuleFor(es => es.EvaluationFinishedDate, f => null)
            .RuleFor(es => es.ReportFile, f => null)
            .RuleFor(es => es.WeightedScore, f => null)
            .Generate(10);

        modelBuilder.Entity<EvaluationSession>().HasData(evaluationSessions);

        var recommendations = new Faker<Recommendation>()
            .UseSeed(seed)
            .RuleFor(r => r.Id, f => f.IndexFaker + 1)
            .RuleFor(r => r.EmployeeId, f => f.PickRandom(employees).Id)
            .RuleFor(r => r.IsVisibleToEmployee, f => f.Random.Bool(0.5f))
            .RuleFor(r => r.RecommendationText, f => f.Lorem.Paragraph())
            .RuleFor(r => r.CreatedAt, f => f.Date.Past(1, now))
            .Generate(10);

        modelBuilder.Entity<Recommendation>().HasData(recommendations);
    }
}
