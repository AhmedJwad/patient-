using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.API.Migrations
{
    public partial class mobilephone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_histories_patients_patientId",
                table: "histories");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_bloods_bloodId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_BloodTypes_bloodTypeId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_Cities_CityId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_gendres_gendreId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_natianalities_NatianalityId",
                table: "patients");

            migrationBuilder.DropTable(
                name: "bloods");

            migrationBuilder.DropIndex(
                name: "IX_patients_bloodId",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "bloodId",
                table: "patients");

            migrationBuilder.AlterColumn<int>(
                name: "gendreId",
                table: "patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "bloodTypeId",
                table: "patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NatianalityId",
                table: "patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MobilePhone",
                table: "patients",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "patients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "surgeries",
                table: "histories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "patientId",
                table: "histories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "histories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "details",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_histories_patients_patientId",
                table: "histories",
                column: "patientId",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_BloodTypes_bloodTypeId",
                table: "patients",
                column: "bloodTypeId",
                principalTable: "BloodTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_Cities_CityId",
                table: "patients",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_gendres_gendreId",
                table: "patients",
                column: "gendreId",
                principalTable: "gendres",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_natianalities_NatianalityId",
                table: "patients",
                column: "NatianalityId",
                principalTable: "natianalities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_histories_patients_patientId",
                table: "histories");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_BloodTypes_bloodTypeId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_Cities_CityId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_gendres_gendreId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_natianalities_NatianalityId",
                table: "patients");

            migrationBuilder.AlterColumn<int>(
                name: "gendreId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "bloodTypeId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NatianalityId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobilePhone",
                table: "patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "bloodId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "surgeries",
                table: "histories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "patientId",
                table: "histories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "histories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "details",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
                name: "FK_histories_patients_patientId",
                table: "histories",
                column: "patientId",
                principalTable: "patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_bloods_bloodId",
                table: "patients",
                column: "bloodId",
                principalTable: "bloods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_BloodTypes_bloodTypeId",
                table: "patients",
                column: "bloodTypeId",
                principalTable: "BloodTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_Cities_CityId",
                table: "patients",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_gendres_gendreId",
                table: "patients",
                column: "gendreId",
                principalTable: "gendres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_natianalities_NatianalityId",
                table: "patients",
                column: "NatianalityId",
                principalTable: "natianalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
