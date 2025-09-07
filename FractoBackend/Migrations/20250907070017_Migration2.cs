using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FractoBackend.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_User",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctor_Specialization",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Doctor",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_User",
                table: "Ratings");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_User",
                table: "Appointments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctor_Specialization",
                table: "Doctors",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "SpecializationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Doctor",
                table: "Ratings",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_User",
                table: "Ratings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_User",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctor_Specialization",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Doctor",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_User",
                table: "Ratings");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_User",
                table: "Appointments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctor_Specialization",
                table: "Doctors",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "SpecializationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Doctor",
                table: "Ratings",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_User",
                table: "Ratings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
