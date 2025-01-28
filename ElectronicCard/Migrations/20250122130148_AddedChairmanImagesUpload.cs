using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class AddedChairmanImagesUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChairmanImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChairmanAccountId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NationalIdFrontPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalIdBackPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelfiePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChairmanImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChairmanImages_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChairmanImages_ApplicationUserId",
                table: "ChairmanImages",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChairmanImages");
        }
    }
}
