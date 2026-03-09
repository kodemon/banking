using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Atomic.Database.Migrations
{
    /// <inheritdoc />
    public partial class Atomicinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "atomic");

            migrationBuilder.CreateTable(
                name: "AtomicRecords",
                schema: "atomic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FlowId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskName = table.Column<string>(type: "text", nullable: false),
                    RollbackJson = table.Column<string>(type: "text", nullable: false),
                    RollbackType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtomicRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtomicRecords_FlowId",
                schema: "atomic",
                table: "AtomicRecords",
                column: "FlowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtomicRecords",
                schema: "atomic");
        }
    }
}
