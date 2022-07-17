using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.API.Migrations
{
    public partial class blood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MobilePhone",
                table: "patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "bloodId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "bloods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bloods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patients_bloodId",
                table: "patients",
                column: "bloodId");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_bloods_bloodId",
                table: "patients",
                column: "bloodId",
                principalTable: "bloods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patients_bloods_bloodId",
                table: "patients");

            migrationBuilder.DropTable(
                name: "bloods");

            migrationBuilder.DropIndex(
                name: "IX_patients_bloodId",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "bloodId",
                table: "patients");

            migrationBuilder.AlterColumn<string>(
                name: "MobilePhone",
                table: "patients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
