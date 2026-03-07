using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.AtomicFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Atomicflowinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtomicFlowRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TaskName = table.Column<string>(type: "TEXT", nullable: false),
                    RollbackJson = table.Column<string>(type: "TEXT", nullable: false),
                    RollbackType = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtomicFlowRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtomicFlowRecords_FlowId",
                table: "AtomicFlowRecords",
                column: "FlowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtomicFlowRecords");
        }
    }
}
