using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStay.API.Migrations
{
    /// <inheritdoc />
    public partial class Addeddefaultapikey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys",
                column: "Key",
                unique: true);
        }
    }
}
