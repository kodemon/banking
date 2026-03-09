using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Principals.Database.Migrations
{
    /// <inheritdoc />
    public partial class Principalsinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "principals");

            migrationBuilder.CreateTable(
                name: "Principals",
                schema: "principals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Attributes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Principals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrincipalIdentities",
                schema: "principals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrincipalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalIdentities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrincipalIdentities_Principals_PrincipalId",
                        column: x => x.PrincipalId,
                        principalSchema: "principals",
                        principalTable: "Principals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrincipalRoles",
                schema: "principals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrincipalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrincipalRoles_Principals_PrincipalId",
                        column: x => x.PrincipalId,
                        principalSchema: "principals",
                        principalTable: "Principals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalIdentities_PrincipalId",
                schema: "principals",
                table: "PrincipalIdentities",
                column: "PrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalIdentities_Provider_ExternalId",
                schema: "principals",
                table: "PrincipalIdentities",
                columns: new[] { "Provider", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalRoles_PrincipalId_Role",
                schema: "principals",
                table: "PrincipalRoles",
                columns: new[] { "PrincipalId", "Role" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrincipalIdentities",
                schema: "principals");

            migrationBuilder.DropTable(
                name: "PrincipalRoles",
                schema: "principals");

            migrationBuilder.DropTable(
                name: "Principals",
                schema: "principals");
        }
    }
}
