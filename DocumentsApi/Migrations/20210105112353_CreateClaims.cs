using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentsApi.Migrations
{
    public partial class CreateClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "claims",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    document_id = table.Column<Guid>(nullable: false),
                    service_area_created_by = table.Column<string>(nullable: true),
                    user_created_by = table.Column<string>(nullable: true),
                    api_created_by = table.Column<string>(nullable: false),
                    retention_expires_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_claims_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_claims_document_id",
                table: "claims",
                column: "document_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claims");
        }
    }
}
