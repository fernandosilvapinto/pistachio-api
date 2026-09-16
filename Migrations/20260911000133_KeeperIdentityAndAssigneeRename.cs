using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pistachio.api.Migrations
{
    /// <inheritdoc />
    public partial class AnvilIdentityAndAssigneeRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedulings_Users_AssignedMechanicId",
                table: "Schedulings");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordResetTokenExpiresAt",
                table: "Users",
                newName: "LastSeenAt");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "AssignedMechanicId",
                table: "Schedulings",
                newName: "AssigneeId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedulings_AssignedMechanicId",
                table: "Schedulings",
                newName: "IX_Schedulings_AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Subject",
                table: "Users",
                column: "Subject",
                unique: true,
                filter: "\"Subject\" <> ''");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedulings_Users_AssigneeId",
                table: "Schedulings",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedulings_Users_AssigneeId",
                table: "Schedulings");

            migrationBuilder.DropIndex(
                name: "IX_Users_Subject",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "LastSeenAt",
                table: "Users",
                newName: "PasswordResetTokenExpiresAt");

            migrationBuilder.RenameColumn(
                name: "AssigneeId",
                table: "Schedulings",
                newName: "AssignedMechanicId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedulings_AssigneeId",
                table: "Schedulings",
                newName: "IX_Schedulings_AssignedMechanicId");

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedulings_Users_AssignedMechanicId",
                table: "Schedulings",
                column: "AssignedMechanicId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
