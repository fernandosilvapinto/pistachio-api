using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pistachio.api.Migrations
{
    /// <inheritdoc />
    public partial class AjustePayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchedulingId",
                table: "Payments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SchedulingId",
                table: "Payments",
                column: "SchedulingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Schedulings_SchedulingId",
                table: "Payments",
                column: "SchedulingId",
                principalTable: "Schedulings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Schedulings_SchedulingId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SchedulingId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SchedulingId",
                table: "Payments");
        }
    }
}
