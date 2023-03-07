using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWeb.Migrations
{
    public partial class AddSattelmentInOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SettelmentReceipts_OrderId",
                table: "SettelmentReceipts");

            migrationBuilder.CreateIndex(
                name: "IX_SettelmentReceipts_OrderId",
                table: "SettelmentReceipts",
                column: "OrderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SettelmentReceipts_OrderId",
                table: "SettelmentReceipts");

            migrationBuilder.CreateIndex(
                name: "IX_SettelmentReceipts_OrderId",
                table: "SettelmentReceipts",
                column: "OrderId");
        }
    }
}
