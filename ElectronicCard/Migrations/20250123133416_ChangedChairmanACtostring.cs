using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class ChangedChairmanACtostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChairmanAccountId",
                table: "ChairmanImages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ChairmanAccountId",
                table: "ChairmanImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
