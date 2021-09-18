using Microsoft.EntityFrameworkCore.Migrations;

namespace Find_Me_Mobile.Migrations
{
    public partial class DeviceDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Companies_CompanyId",
                table: "Devices");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Devices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Audio",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AverageRating",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bluetooth",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuiltIn",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CPU",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Capacity",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Card",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Chipset",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Colors",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dimensions",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extra",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraFeatures",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Features",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Front",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G2Band",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G3Band",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G4Band",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G5Band",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GPS",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GPU",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Games",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Main",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Messaging",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NFC",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OS",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Protection",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SIM",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sensors",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Technology",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Torch",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UI",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USB",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WLAN",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Weight",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Companies_CompanyId",
                table: "Devices",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Companies_CompanyId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Audio",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Bluetooth",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Browser",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "BuiltIn",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "CPU",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Card",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Chipset",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Colors",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Dimensions",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Extra",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ExtraFeatures",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Features",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Front",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "G2Band",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "G3Band",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "G4Band",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "G5Band",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "GPS",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "GPU",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Games",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Main",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Messaging",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "NFC",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "OS",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Protection",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "SIM",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Sensors",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Technology",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Torch",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "UI",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "USB",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WLAN",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "DeviceDetails");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Devices",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Companies_CompanyId",
                table: "Devices",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
