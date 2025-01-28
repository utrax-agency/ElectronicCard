using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class FisrtLastNameInGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Treasurer_Name",
                table: "Groups",
                newName: "Treasurer_LastName");

            migrationBuilder.AddColumn<string>(
                name: "Treasurer_FirstName",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Treasurer_FirstName",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "Treasurer_LastName",
                table: "Groups",
                newName: "Treasurer_Name");
        }
    }
}
