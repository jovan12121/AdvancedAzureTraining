using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTasks.Migrations
{
    /// <inheritdoc />
    public partial class migrationMetaData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaData",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaData",
                table: "Tasks");
        }
    }
}
