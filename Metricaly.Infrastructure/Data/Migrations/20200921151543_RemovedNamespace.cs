using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Metricaly.Infrastructure.Data.Migrations
{
    public partial class RemovedNamespace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metrics_Namespaces_NamespaceId",
                table: "Metrics");

            migrationBuilder.DropTable(
                name: "Namespaces");

            migrationBuilder.DropIndex(
                name: "IX_Metrics_NamespaceId",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "NamespaceId",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "ShortKey",
                table: "Applications");

            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "Metrics",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Namespace",
                table: "Metrics",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_ApplicationId",
                table: "Metrics",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Metrics_Applications_ApplicationId",
                table: "Metrics",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metrics_Applications_ApplicationId",
                table: "Metrics");

            migrationBuilder.DropIndex(
                name: "IX_Metrics_ApplicationId",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "Namespace",
                table: "Metrics");

            migrationBuilder.AddColumn<long>(
                name: "NamespaceId",
                table: "Metrics",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ShortKey",
                table: "Applications",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Namespaces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Namespaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Namespaces_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_NamespaceId",
                table: "Metrics",
                column: "NamespaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Namespaces_ApplicationId",
                table: "Namespaces",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Metrics_Namespaces_NamespaceId",
                table: "Metrics",
                column: "NamespaceId",
                principalTable: "Namespaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
