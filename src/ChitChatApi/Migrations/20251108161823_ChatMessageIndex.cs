using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChitChatApi.Migrations
{
    /// <inheritdoc />
    public partial class ChatMessageIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_Chatroom_Id",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "ChatMessages",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_Chatroom_Id_Id",
                table: "ChatMessages",
                columns: new[] { "Chatroom_Id", "Id" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_Chatroom_Id_Id",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "ChatMessages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "now()");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_Chatroom_Id",
                table: "ChatMessages",
                column: "Chatroom_Id");
        }
    }
}
