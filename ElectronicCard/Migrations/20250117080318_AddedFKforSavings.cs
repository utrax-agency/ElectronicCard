using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class AddedFKforSavings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Savings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Member_id",
                table: "Savings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Savings_MemberId",
                table: "Savings",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Savings_Members_MemberId",
                table: "Savings",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Savings_Members_MemberId",
                table: "Savings");

            migrationBuilder.DropIndex(
                name: "IX_Savings_MemberId",
                table: "Savings");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Savings");

            migrationBuilder.DropColumn(
                name: "Member_id",
                table: "Savings");
        }
    }
}
