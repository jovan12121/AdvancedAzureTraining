using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTasks.Migrations
{
    /// <inheritdoc />
    public partial class AddedFinishingDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinished",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinished",
                table: "Projects",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateFinished",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DateFinished",
                table: "Projects");
        }
    }
}
