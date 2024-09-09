using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.Migrations
{
    /// <inheritdoc />
    public partial class nnn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTicketTypes_Events_EventId",
                table: "EventTicketTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTicketTypes_TicketTypes_TicketTypeId",
                table: "EventTicketTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventTicketTypes",
                table: "EventTicketTypes");

            migrationBuilder.RenameTable(
                name: "EventTicketTypes",
                newName: "EventTicketType");

            migrationBuilder.RenameIndex(
                name: "IX_EventTicketTypes_TicketTypeId",
                table: "EventTicketType",
                newName: "IX_EventTicketType_TicketTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EventTicketTypes_EventId",
                table: "EventTicketType",
                newName: "IX_EventTicketType_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventTicketType",
                table: "EventTicketType",
                column: "EventTicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTicketType_Events_EventId",
                table: "EventTicketType",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTicketType_TicketTypes_TicketTypeId",
                table: "EventTicketType",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "TicketTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTicketType_Events_EventId",
                table: "EventTicketType");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTicketType_TicketTypes_TicketTypeId",
                table: "EventTicketType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventTicketType",
                table: "EventTicketType");

            migrationBuilder.RenameTable(
                name: "EventTicketType",
                newName: "EventTicketTypes");

            migrationBuilder.RenameIndex(
                name: "IX_EventTicketType_TicketTypeId",
                table: "EventTicketTypes",
                newName: "IX_EventTicketTypes_TicketTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EventTicketType_EventId",
                table: "EventTicketTypes",
                newName: "IX_EventTicketTypes_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventTicketTypes",
                table: "EventTicketTypes",
                column: "EventTicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTicketTypes_Events_EventId",
                table: "EventTicketTypes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTicketTypes_TicketTypes_TicketTypeId",
                table: "EventTicketTypes",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "TicketTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
