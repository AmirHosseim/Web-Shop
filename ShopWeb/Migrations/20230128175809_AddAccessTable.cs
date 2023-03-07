using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWeb.Migrations
{
    public partial class AddAccessTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessToEmploys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessToEmploys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessToEmploys_Accesses_AccessId",
                        column: x => x.AccessId,
                        principalTable: "Accesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessToEmploys_AccessId",
                table: "AccessToEmploys",
                column: "AccessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessToEmploys");

            migrationBuilder.DropTable(
                name: "Accesses");
        }
    }
}
