using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicCard.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Provinces_ProvinceId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Provinces_Province_ID",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Groups_GroupId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_GroupId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ProvinceId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Group_id",
                table: "Members",
                column: "Group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Provinces_Province_ID",
                table: "Groups",
                column: "Province_ID",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Groups_Group_id",
                table: "Members",
                column: "Group_id",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Provinces_Province_ID",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Groups_Group_id",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_Group_id",
                table: "Members");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_GroupId",
                table: "Members",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ProvinceId",
                table: "Groups",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Provinces_ProvinceId",
                table: "Groups",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Provinces_Province_ID",
                table: "Groups",
                column: "Province_ID",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Groups_GroupId",
                table: "Members",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
