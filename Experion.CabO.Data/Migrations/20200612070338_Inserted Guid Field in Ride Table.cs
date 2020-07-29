using Microsoft.EntityFrameworkCore.Migrations;

namespace Experion.CabO.Data.Migrations
{
    public partial class InsertedGuidFieldinRideTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "Ride",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Ride");
        }
    }
}
