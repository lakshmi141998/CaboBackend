using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Experion.CabO.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cab",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(maxLength: 50, nullable: true),
                    Capacity = table.Column<int>(nullable: false),
                    VehicleNo = table.Column<string>(maxLength: 30, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Driver",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNo = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficeLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RideRequestor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideRequestor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RideStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RideType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rideType = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftName = table.Column<string>(maxLength: 50, nullable: true),
                    ShiftStart = table.Column<DateTime>(nullable: false),
                    ShiftEnd = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficeCommutation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location1 = table.Column<int>(nullable: false),
                    Location2 = table.Column<int>(nullable: false),
                    CabId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeCommutation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeCommutation_Cab_CabId",
                        column: x => x.CabId,
                        principalTable: "Cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RideAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CabId = table.Column<int>(nullable: false),
                    DriverId = table.Column<int>(nullable: false),
                    DailyDate = table.Column<DateTime>(nullable: false),
                    InitialReading = table.Column<int>(nullable: true),
                    FinalReading = table.Column<int>(nullable: true),
                    ShiftId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RideAssignment_Cab_CabId",
                        column: x => x.CabId,
                        principalTable: "Cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RideAssignment_Driver_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RideAssignment_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvailableTime",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    OfficeCommutationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailableTime_OfficeCommutation_OfficeCommutationId",
                        column: x => x.OfficeCommutationId,
                        principalTable: "OfficeCommutation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ride",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RideDate = table.Column<DateTime>(nullable: false),
                    RideTime = table.Column<DateTime>(nullable: false),
                    RideRequestorId = table.Column<int>(nullable: false),
                    From = table.Column<string>(maxLength: 100, nullable: true),
                    To = table.Column<string>(maxLength: 100, nullable: true),
                    RideAssignmentId = table.Column<int>(nullable: true),
                    InitialReading = table.Column<long>(nullable: true),
                    FinalReading = table.Column<long>(nullable: true),
                    ContactNo = table.Column<string>(maxLength: 10, nullable: true),
                    PassengerCount = table.Column<int>(nullable: false),
                    CabType = table.Column<string>(maxLength: 10, nullable: true),
                    Purpose = table.Column<string>(maxLength: 100, nullable: true),
                    ProjectCode = table.Column<string>(maxLength: 150, nullable: true),
                    ExternalCabName = table.Column<string>(maxLength: 50, nullable: true),
                    CancelReason = table.Column<string>(maxLength: 100, nullable: true),
                    RideStatusId = table.Column<int>(nullable: true),
                    RideTypeId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ride", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ride_RideAssignment_RideAssignmentId",
                        column: x => x.RideAssignmentId,
                        principalTable: "RideAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ride_RideRequestor_RideRequestorId",
                        column: x => x.RideRequestorId,
                        principalTable: "RideRequestor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ride_RideStatus_RideStatusId",
                        column: x => x.RideStatusId,
                        principalTable: "RideStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ride_RideType_RideTypeId",
                        column: x => x.RideTypeId,
                        principalTable: "RideType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RideId = table.Column<int>(nullable: true),
                    Timing = table.Column<float>(nullable: true),
                    Behaviour = table.Column<float>(nullable: true),
                    Overall = table.Column<float>(nullable: true),
                    Comments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rating_Ride_RideId",
                        column: x => x.RideId,
                        principalTable: "Ride",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "RideStatus",
                columns: new[] { "Id", "StatusName" },
                values: new object[,]
                {
                    { 1, "Approved" },
                    { 2, "Pending" },
                    { 3, "Rejected" },
                    { 4, "Completed" },
                    { 5, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "RideType",
                columns: new[] { "Id", "rideType" },
                values: new object[,]
                {
                    { 1, "CABO-OFFICE" },
                    { 2, "CABO-CLIENT" },
                    { 3, "CABO-OTHERS" },
                    { 4, "CABO-ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Shift",
                columns: new[] { "Id", "ShiftEnd", "ShiftName", "ShiftStart" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 17, 30, 0, 0, DateTimeKind.Unspecified), "Day", new DateTime(1, 1, 1, 7, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(1, 1, 1, 7, 30, 0, 0, DateTimeKind.Unspecified), "Night", new DateTime(1, 1, 1, 17, 30, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableTime_OfficeCommutationId",
                table: "AvailableTime",
                column: "OfficeCommutationId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeCommutation_CabId",
                table: "OfficeCommutation",
                column: "CabId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_RideId",
                table: "Rating",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_RideAssignmentId",
                table: "Ride",
                column: "RideAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_RideRequestorId",
                table: "Ride",
                column: "RideRequestorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_RideStatusId",
                table: "Ride",
                column: "RideStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_RideTypeId",
                table: "Ride",
                column: "RideTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RideAssignment_CabId",
                table: "RideAssignment",
                column: "CabId");

            migrationBuilder.CreateIndex(
                name: "IX_RideAssignment_DriverId",
                table: "RideAssignment",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RideAssignment_ShiftId",
                table: "RideAssignment",
                column: "ShiftId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailableTime");

            migrationBuilder.DropTable(
                name: "OfficeLocation");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "OfficeCommutation");

            migrationBuilder.DropTable(
                name: "Ride");

            migrationBuilder.DropTable(
                name: "RideAssignment");

            migrationBuilder.DropTable(
                name: "RideRequestor");

            migrationBuilder.DropTable(
                name: "RideStatus");

            migrationBuilder.DropTable(
                name: "RideType");

            migrationBuilder.DropTable(
                name: "Cab");

            migrationBuilder.DropTable(
                name: "Driver");

            migrationBuilder.DropTable(
                name: "Shift");
        }
    }
}
