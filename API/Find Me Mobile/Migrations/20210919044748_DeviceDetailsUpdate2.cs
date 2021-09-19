using Microsoft.EntityFrameworkCore.Migrations;

namespace Find_Me_Mobile.Migrations
{
    public partial class DeviceDetailsUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OS",
                table: "DeviceDetails",
                newName: "WiFi");

            migrationBuilder.AddColumn<string>(
                name: "Accelerometer",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmbientLightSensor",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Aperture",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AudioPlayback",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Charging",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ECompass",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fingerprint",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flash",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GyroscopeSensor",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OTG",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatingSystem",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Processor",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProximitySensor",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ram",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rom",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SIMSlotType",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SceneModes",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Screen",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StandbyMode",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TouchScreen",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoPlayback",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoRecording",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoiceRecording",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accelerometer",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "AmbientLightSensor",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Aperture",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "AudioPlayback",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Charging",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ECompass",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Fingerprint",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Flash",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "GyroscopeSensor",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "OTG",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "OperatingSystem",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Processor",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ProximitySensor",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Ram",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Rom",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "SIMSlotType",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "SceneModes",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Screen",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "StandbyMode",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "TouchScreen",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "VideoPlayback",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "VideoRecording",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "VoiceRecording",
                table: "DeviceDetails");

            migrationBuilder.RenameColumn(
                name: "WiFi",
                table: "DeviceDetails",
                newName: "OS");
        }
    }
}
