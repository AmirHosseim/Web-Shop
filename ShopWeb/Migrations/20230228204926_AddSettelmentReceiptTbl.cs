using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWeb.Migrations
{
    public partial class AddSettelmentReceiptTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SettelmentReceipts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettelmentReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettelmentReceipts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SettelmentReceipts_OrderId",
                table: "SettelmentReceipts",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettelmentReceipts");
        }
    }
}
