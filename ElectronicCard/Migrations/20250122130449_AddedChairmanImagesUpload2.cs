using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class AddedChairmanImagesUpload2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChairmanImages_AspNetUsers_ApplicationUserId",
                table: "ChairmanImages");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ChairmanImages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ChairmanImages_AspNetUsers_ApplicationUserId",
                table: "ChairmanImages",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChairmanImages_AspNetUsers_ApplicationUserId",
                table: "ChairmanImages");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ChairmanImages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChairmanImages_AspNetUsers_ApplicationUserId",
                table: "ChairmanImages",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
