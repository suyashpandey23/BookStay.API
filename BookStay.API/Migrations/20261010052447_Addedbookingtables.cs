using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStay.API.Migrations
{
    /// <inheritdoc />
    public partial class Addedbookingtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PerNightCharge",
                table: "Stays",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StayId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CheckIn = table.Column<DateOnly>(type: "date", nullable: false),
                    CheckOut = table.Column<DateOnly>(type: "date", nullable: false),
                    Guests = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Stays_StayId",
                        column: x => x.StayId,
                        principalTable: "Stays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StayAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StayId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StayAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StayAdmins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StayAdmins_Stays_StayId",
                        column: x => x.StayId,
                        principalTable: "Stays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CheckIn_CheckOut",
                table: "Bookings",
                columns: new[] { "CheckIn", "CheckOut" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StayId",
                table: "Bookings",
                column: "StayId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StayAdmins_StayId",
                table: "StayAdmins",
                column: "StayId");

            migrationBuilder.CreateIndex(
                name: "IX_StayAdmins_UserId",
                table: "StayAdmins",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "StayAdmins");

            migrationBuilder.DropColumn(
                name: "PerNightCharge",
                table: "Stays");
        }
    }
}
