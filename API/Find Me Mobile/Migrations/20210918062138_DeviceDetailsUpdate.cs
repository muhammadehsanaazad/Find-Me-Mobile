using Microsoft.EntityFrameworkCore.Migrations;

namespace Find_Me_Mobile.Migrations
{
    public partial class DeviceDetailsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceDetails_DeviceId",
                table: "DeviceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDetails_DeviceId",
                table: "DeviceDetails",
                column: "DeviceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceDetails_DeviceId",
                table: "DeviceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDetails_DeviceId",
                table: "DeviceDetails",
                column: "DeviceId");
        }
    }
}
