using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pistachio.api.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedMechanicToScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedulings_Users_UserId",
                table: "Schedulings");

            migrationBuilder.AddColumn<int>(
                name: "AssignedMechanicId",
                table: "Schedulings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedulings_AssignedMechanicId",
                table: "Schedulings",
                column: "AssignedMechanicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedulings_Users_AssignedMechanicId",
                table: "Schedulings",
                column: "AssignedMechanicId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedulings_Users_UserId",
                table: "Schedulings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedulings_Users_AssignedMechanicId",
                table: "Schedulings");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedulings_Users_UserId",
                table: "Schedulings");

            migrationBuilder.DropIndex(
                name: "IX_Schedulings_AssignedMechanicId",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "AssignedMechanicId",
                table: "Schedulings");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedulings_Users_UserId",
                table: "Schedulings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
