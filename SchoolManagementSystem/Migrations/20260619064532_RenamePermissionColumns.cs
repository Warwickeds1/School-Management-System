using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenamePermissionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanEdit",
                table: "RolePermissions",
                newName: "CanUpdate");

            migrationBuilder.RenameColumn(
                name: "CanDelete",
                table: "RolePermissions",
                newName: "CanAdd");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanUpdate",
                table: "RolePermissions",
                newName: "CanEdit");

            migrationBuilder.RenameColumn(
                name: "CanAdd",
                table: "RolePermissions",
                newName: "CanDelete");
        }
    }
}
