using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWeb.Migrations
{
    public partial class AddImagesInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductsImagesInfo",
                columns: table => new
                {
                    ImageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsImagesInfo", x => x.ImageId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsImagesInfo");
        }
    }
}
