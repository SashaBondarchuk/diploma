using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee.Performance.Evaluator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDecimalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WeightedScore",
                table: "EvaluationSessions",
                type: "decimal(6,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinScore",
                table: "EmployeeClasses",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxScore",
                table: "EmployeeClasses",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WeightedScore",
                table: "EvaluationSessions",
                type: "decimal(5,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinScore",
                table: "EmployeeClasses",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxScore",
                table: "EmployeeClasses",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");
        }
    }
}
