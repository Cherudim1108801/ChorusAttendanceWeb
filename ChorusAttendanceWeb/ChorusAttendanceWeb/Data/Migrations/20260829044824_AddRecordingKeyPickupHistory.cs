using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChorusAttendanceWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordingKeyPickupHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "KeyPickedUp",
                table: "Practices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresKeyPickup",
                table: "Practices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "PracticePieces",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecordingUrl",
                table: "PracticePieces",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyPickedUp",
                table: "Practices");

            migrationBuilder.DropColumn(
                name: "RequiresKeyPickup",
                table: "Practices");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "PracticePieces");

            migrationBuilder.DropColumn(
                name: "RecordingUrl",
                table: "PracticePieces");
        }
    }
}
