using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.Migrations
{
    /// <inheritdoc />
    public partial class TicketTypeEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTicketType",
                columns: table => new
                {
                    EventsEventId = table.Column<int>(type: "int", nullable: false),
                    TicketTypesTicketTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTicketType", x => new { x.EventsEventId, x.TicketTypesTicketTypeId });
                    table.ForeignKey(
                        name: "FK_EventTicketType_Events_EventsEventId",
                        column: x => x.EventsEventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTicketType_TicketTypes_TicketTypesTicketTypeId",
                        column: x => x.TicketTypesTicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "TicketTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTicketType_TicketTypesTicketTypeId",
                table: "EventTicketType",
                column: "TicketTypesTicketTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTicketType");
        }
    }
}
