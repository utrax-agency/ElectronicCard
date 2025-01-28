using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class FisrtLastNameInGroup2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Secretary_Name",
                table: "Groups",
                newName: "Secretary_LastName");

            migrationBuilder.RenameColumn(
                name: "Chairman_Name",
                table: "Groups",
                newName: "Secretary_FirstName");

            migrationBuilder.AddColumn<string>(
                name: "Chairman_FirstName",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Chairman_LastName",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chairman_FirstName",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Chairman_LastName",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "Secretary_LastName",
                table: "Groups",
                newName: "Secretary_Name");

            migrationBuilder.RenameColumn(
                name: "Secretary_FirstName",
                table: "Groups",
                newName: "Chairman_Name");
        }
    }
}
