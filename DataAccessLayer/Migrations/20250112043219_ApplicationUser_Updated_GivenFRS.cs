using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUser_Updated_GivenFRS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "AspNetUsers",
                newName: "Department");

            migrationBuilder.AddColumn<int>(
                name: "Pin",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pin",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "AspNetUsers",
                newName: "Gender");
        }
    }
}
