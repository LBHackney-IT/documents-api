using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentsApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTargetIdToGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "target_id",
                table: "claims",
                newName: "group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "group_id",
                table: "claims",
                newName: "target_id");
        }
    }
}
