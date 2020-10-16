using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProffyBackend.Migrations
{
    public partial class ProffyAvailabilityFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AvailableTimeWindows",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    StartHour = table.Column<int>(nullable: false),
                    EndHour = table.Column<int>(nullable: false),
                    WeekDay = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableTimeWindows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailableTimeWindows_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableTimeWindows_OwnerId",
                table: "AvailableTimeWindows",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableTimeWindows_WeekDay_OwnerId",
                table: "AvailableTimeWindows",
                columns: new[] { "WeekDay", "OwnerId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailableTimeWindows");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");
        }
    }
}
