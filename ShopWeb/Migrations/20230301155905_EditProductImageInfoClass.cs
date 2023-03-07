using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWeb.Migrations
{
    public partial class EditProductImageInfoClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductsImagesInfo",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsImagesInfo_ProductId",
                table: "ProductsImagesInfo",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsImagesInfo_Products_ProductId",
                table: "ProductsImagesInfo",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsImagesInfo_Products_ProductId",
                table: "ProductsImagesInfo");

            migrationBuilder.DropIndex(
                name: "IX_ProductsImagesInfo_ProductId",
                table: "ProductsImagesInfo");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductsImagesInfo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
