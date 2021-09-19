using Microsoft.EntityFrameworkCore.Migrations;

namespace Find_Me_Mobile.Migrations
{
    public partial class DeviceDetailsUpdate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceImages");

            migrationBuilder.RenameColumn(
                name: "PrimaryImage",
                table: "Devices",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "Main",
                table: "DeviceDetails",
                newName: "FM");

            migrationBuilder.RenameColumn(
                name: "Front",
                table: "DeviceDetails",
                newName: "Camera");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Devices",
                newName: "PrimaryImage");

            migrationBuilder.RenameColumn(
                name: "FM",
                table: "DeviceDetails",
                newName: "Main");

            migrationBuilder.RenameColumn(
                name: "Camera",
                table: "DeviceDetails",
                newName: "Front");

            migrationBuilder.CreateTable(
                name: "DeviceImages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceImages_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceImages_DeviceId",
                table: "DeviceImages",
                column: "DeviceId");
        }
    }
}
