using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.API.Migrations
{
    public partial class agend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agenda_AspNetUsers_userId",
                table: "Agenda");

            migrationBuilder.DropForeignKey(
                name: "FK_Agenda_patients_pathientId",
                table: "Agenda");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_UserPatient_userPatientId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPatient_AspNetUsers_UserId",
                table: "UserPatient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPatient",
                table: "UserPatient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agenda",
                table: "Agenda");

            migrationBuilder.RenameTable(
                name: "UserPatient",
                newName: "UserPatients");

            migrationBuilder.RenameTable(
                name: "Agenda",
                newName: "agendas");

            migrationBuilder.RenameIndex(
                name: "IX_UserPatient_UserId",
                table: "UserPatients",
                newName: "IX_UserPatients_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Agenda_userId",
                table: "agendas",
                newName: "IX_agendas_userId");

            migrationBuilder.RenameIndex(
                name: "IX_Agenda_pathientId",
                table: "agendas",
                newName: "IX_agendas_pathientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPatients",
                table: "UserPatients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_agendas",
                table: "agendas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_agendas_AspNetUsers_userId",
                table: "agendas",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_agendas_patients_pathientId",
                table: "agendas",
                column: "pathientId",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_UserPatients_userPatientId",
                table: "patients",
                column: "userPatientId",
                principalTable: "UserPatients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPatients_AspNetUsers_UserId",
                table: "UserPatients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_agendas_AspNetUsers_userId",
                table: "agendas");

            migrationBuilder.DropForeignKey(
                name: "FK_agendas_patients_pathientId",
                table: "agendas");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_UserPatients_userPatientId",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPatients_AspNetUsers_UserId",
                table: "UserPatients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPatients",
                table: "UserPatients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_agendas",
                table: "agendas");

            migrationBuilder.RenameTable(
                name: "UserPatients",
                newName: "UserPatient");

            migrationBuilder.RenameTable(
                name: "agendas",
                newName: "Agenda");

            migrationBuilder.RenameIndex(
                name: "IX_UserPatients_UserId",
                table: "UserPatient",
                newName: "IX_UserPatient_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_agendas_userId",
                table: "Agenda",
                newName: "IX_Agenda_userId");

            migrationBuilder.RenameIndex(
                name: "IX_agendas_pathientId",
                table: "Agenda",
                newName: "IX_Agenda_pathientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPatient",
                table: "UserPatient",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agenda",
                table: "Agenda",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agenda_AspNetUsers_userId",
                table: "Agenda",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agenda_patients_pathientId",
                table: "Agenda",
                column: "pathientId",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_UserPatient_userPatientId",
                table: "patients",
                column: "userPatientId",
                principalTable: "UserPatient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPatient_AspNetUsers_UserId",
                table: "UserPatient",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
