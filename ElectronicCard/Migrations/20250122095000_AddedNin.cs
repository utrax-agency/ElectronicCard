using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class AddedNin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NIN",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NIN",
                table: "Members");
        }
    }
}
