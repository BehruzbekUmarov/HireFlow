using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HireFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUrlToFreelancerCv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvUrl",
                table: "users");

            migrationBuilder.AddColumn<long>(
                name: "CvId",
                table: "job_applications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "freelancer_cvs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Skills = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Experience = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Education = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Languages = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PortfolioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_freelancer_cvs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_freelancer_cvs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_CvId",
                table: "job_applications",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_freelancer_cvs_UserId",
                table: "freelancer_cvs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_freelancer_cvs_UserId_IsDefault",
                table: "freelancer_cvs",
                columns: new[] { "UserId", "IsDefault" });

            migrationBuilder.AddForeignKey(
                name: "FK_job_applications_freelancer_cvs_CvId",
                table: "job_applications",
                column: "CvId",
                principalTable: "freelancer_cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_applications_freelancer_cvs_CvId",
                table: "job_applications");

            migrationBuilder.DropTable(
                name: "freelancer_cvs");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_CvId",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "CvId",
                table: "job_applications");

            migrationBuilder.AddColumn<string>(
                name: "CvUrl",
                table: "users",
                type: "text",
                nullable: true);
        }
    }
}
