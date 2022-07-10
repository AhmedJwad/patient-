using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.API.Migrations
{
    public partial class diagnosictable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "diagonisics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diagonisics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_diagonisics_Description",
                table: "diagonisics",
                column: "Description",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "diagonisics");
        }
    }
}
