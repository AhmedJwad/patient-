using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.API.Migrations
{
    public partial class completetheDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "gendreId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "histories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    allergies = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    illnesses = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    surgeries = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    patientId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_histories_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_histories_patients_patientId",
                        column: x => x.patientId,
                        principalTable: "patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    diagonisicId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_details_diagonisics_diagonisicId",
                        column: x => x.diagonisicId,
                        principalTable: "diagonisics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_details_histories_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "histories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patients_gendreId",
                table: "patients",
                column: "gendreId");

            migrationBuilder.CreateIndex(
                name: "IX_details_diagonisicId",
                table: "details",
                column: "diagonisicId");

            migrationBuilder.CreateIndex(
                name: "IX_details_HistoryId",
                table: "details",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_histories_patientId",
                table: "histories",
                column: "patientId");

            migrationBuilder.CreateIndex(
                name: "IX_histories_userId",
                table: "histories",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_gendres_gendreId",
                table: "patients",
                column: "gendreId",
                principalTable: "gendres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patients_gendres_gendreId",
                table: "patients");

            migrationBuilder.DropTable(
                name: "details");

            migrationBuilder.DropTable(
                name: "histories");

            migrationBuilder.DropIndex(
                name: "IX_patients_gendreId",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "gendreId",
                table: "patients");
        }
    }
}
