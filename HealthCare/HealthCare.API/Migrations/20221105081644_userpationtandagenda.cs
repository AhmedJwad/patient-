using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.API.Migrations
{
    public partial class userpationtandagenda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userPatientId",
                table: "patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Agenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsMine = table.Column<bool>(type: "bit", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    pathientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agenda_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Agenda_patients_pathientId",
                        column: x => x.pathientId,
                        principalTable: "patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPatient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPatient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPatient_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_patients_userPatientId",
                table: "patients",
                column: "userPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Agenda_pathientId",
                table: "Agenda",
                column: "pathientId");

            migrationBuilder.CreateIndex(
                name: "IX_Agenda_userId",
                table: "Agenda",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPatient_UserId",
                table: "UserPatient",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_UserPatient_userPatientId",
                table: "patients",
                column: "userPatientId",
                principalTable: "UserPatient",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patients_UserPatient_userPatientId",
                table: "patients");

            migrationBuilder.DropTable(
                name: "Agenda");

            migrationBuilder.DropTable(
                name: "UserPatient");

            migrationBuilder.DropIndex(
                name: "IX_patients_userPatientId",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "userPatientId",
                table: "patients");
        }
    }
}
