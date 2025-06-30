using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CashFlow.IdentityAndAccess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_user",
                columns: table => new
                {
                    app_user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    app_user_login_name = table.Column<string>(type: "character varying(360)", maxLength: 360, nullable: false),
                    app_user_email = table.Column<string>(type: "character varying(360)", maxLength: 360, nullable: false),
                    app_user_pwd = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    app_user_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user", x => x.app_user_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    role_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    role_desc = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    refresh_token_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    app_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    refresh_token_content = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    refresh_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    refresh_token_is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.refresh_token_id);
                    table.ForeignKey(
                        name: "FK_refresh_token_app_user_app_user_id",
                        column: x => x.app_user_id,
                        principalTable: "app_user",
                        principalColumn: "app_user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    app_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => new { x.app_user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_role_app_user_app_user_id",
                        column: x => x.app_user_id,
                        principalTable: "app_user",
                        principalColumn: "app_user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "role_id", "role_desc", "role_name" },
                values: new object[,]
                {
                    { new Guid("3d6f97b5-f1c2-4bbf-9210-31d7f79e9bba"), "Full system access", "ADMIN" },
                    { new Guid("b2621c5c-7bfa-4b97-a7d3-25417c320f5c"), "General user", "USER" },
                    { new Guid("d416a013-5648-4e81-9e14-5be6ebfa323b"), "Management user", "MANAGER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_user_app_user_email",
                table: "app_user",
                column: "app_user_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_app_user_app_user_login_name",
                table: "app_user",
                column: "app_user_login_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_app_user_id",
                table: "refresh_token",
                column: "app_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_refresh_token_content",
                table: "refresh_token",
                column: "refresh_token_content",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_role_name",
                table: "role",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_role_role_id",
                table: "user_role",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "app_user");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
