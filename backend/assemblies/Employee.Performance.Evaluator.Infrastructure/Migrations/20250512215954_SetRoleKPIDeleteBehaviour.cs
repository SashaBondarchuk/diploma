using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee.Performance.Evaluator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetRoleKPIDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleKPIs_KPIMetrics_KpiId",
                table: "RoleKPIs");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleKPIs_Roles_RoleId",
                table: "RoleKPIs");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleKPIs_KPIMetrics_KpiId",
                table: "RoleKPIs",
                column: "KpiId",
                principalTable: "KPIMetrics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleKPIs_Roles_RoleId",
                table: "RoleKPIs",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleKPIs_KPIMetrics_KpiId",
                table: "RoleKPIs");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleKPIs_Roles_RoleId",
                table: "RoleKPIs");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleKPIs_KPIMetrics_KpiId",
                table: "RoleKPIs",
                column: "KpiId",
                principalTable: "KPIMetrics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleKPIs_Roles_RoleId",
                table: "RoleKPIs",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
