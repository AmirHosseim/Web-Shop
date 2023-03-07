using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWeb.Migrations
{
    public partial class AddRowToProductImageInfoTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ProductsImagesInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "ProductsImagesInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ProductsImagesInfo");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "ProductsImagesInfo");
        }
    }
}
