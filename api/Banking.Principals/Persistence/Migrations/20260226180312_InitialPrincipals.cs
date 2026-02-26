using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Principals.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPrincipals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Principals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Principals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrincipalAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrincipalAttributes_Principals_PrincipalId",
                        column: x => x.PrincipalId,
                        principalTable: "Principals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrincipalIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalIdentities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrincipalIdentities_Principals_PrincipalId",
                        column: x => x.PrincipalId,
                        principalTable: "Principals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrincipalRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrincipalRoles_Principals_PrincipalId",
                        column: x => x.PrincipalId,
                        principalTable: "Principals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalAttributes_PrincipalId_Domain",
                table: "PrincipalAttributes",
                columns: new[] { "PrincipalId", "Domain" });

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalAttributes_PrincipalId_Domain_Key",
                table: "PrincipalAttributes",
                columns: new[] { "PrincipalId", "Domain", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalIdentities_PrincipalId",
                table: "PrincipalIdentities",
                column: "PrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalIdentities_Provider_ExternalId",
                table: "PrincipalIdentities",
                columns: new[] { "Provider", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrincipalRoles_PrincipalId_Role",
                table: "PrincipalRoles",
                columns: new[] { "PrincipalId", "Role" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrincipalAttributes");

            migrationBuilder.DropTable(
                name: "PrincipalIdentities");

            migrationBuilder.DropTable(
                name: "PrincipalRoles");

            migrationBuilder.DropTable(
                name: "Principals");
        }
    }
}
