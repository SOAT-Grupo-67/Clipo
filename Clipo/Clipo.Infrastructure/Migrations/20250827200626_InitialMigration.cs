using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clipo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "videoStatus");

            migrationBuilder.CreateSequence(
                name: "global_id_seq",
                schema: "videoStatus",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "video-status",
                schema: "videoStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('customer.global_id_seq')"),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    ZipPath = table.Column<string>(type: "text", nullable: true),
                    ProcessStatus = table.Column<int>(type: "integer", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_video-status", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "video-status",
                schema: "videoStatus");

            migrationBuilder.DropSequence(
                name: "global_id_seq",
                schema: "videoStatus");
        }
    }
}
