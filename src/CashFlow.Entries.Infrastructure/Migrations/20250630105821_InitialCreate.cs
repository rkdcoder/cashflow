using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CashFlow.Entries.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entry_type",
                columns: table => new
                {
                    entry_type_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    entry_type_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry_type", x => x.entry_type_id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    category_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entry_type_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.category_id);
                    table.ForeignKey(
                        name: "FK_category_entry_type_entry_type_id",
                        column: x => x.entry_type_id,
                        principalTable: "entry_type",
                        principalColumn: "entry_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entry",
                columns: table => new
                {
                    entry_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    entry_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    entry_description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry", x => x.entry_id);
                    table.ForeignKey(
                        name: "FK_entry_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_entry_entry_type_entry_type_id",
                        column: x => x.entry_type_id,
                        principalTable: "entry_type",
                        principalColumn: "entry_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "entry_type",
                columns: new[] { "entry_type_id", "entry_type_name" },
                values: new object[,]
                {
                    { new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "DEBIT" },
                    { new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "CREDIT" }
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "category_id", "entry_type_id", "category_name" },
                values: new object[,]
                {
                    { new Guid("054a551e-9841-4bef-ad35-8bb9a381a916"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "UTILITIES" },
                    { new Guid("0c90948f-f1db-42cb-8b6c-5c43cfa99d41"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "CLOTHING" },
                    { new Guid("283b1aca-ef7a-4fce-bcbb-8752e2153c08"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "FOOD" },
                    { new Guid("47b934b2-e7ec-4dd9-b89e-f542b37505ca"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "LEISURE" },
                    { new Guid("56e7533b-4e48-4483-b387-4aee71e454bd"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "HEALTH" },
                    { new Guid("63eb96fe-2c91-43b9-b415-c65ee867e2bd"), new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "OTHER_CREDITS" },
                    { new Guid("929ed0ff-d817-4a82-8124-3dfcd6a1f9ce"), new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "GIFTS_DONATIONS" },
                    { new Guid("96d8efc8-61f1-4007-9917-0862b0f2f57a"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "HOUSING" },
                    { new Guid("a11d43e6-96d6-415f-b2fe-6f0a55970eb9"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "TRANSPORTATION" },
                    { new Guid("a604b4c1-637c-454a-b4a1-e4a5c1af0c18"), new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "OTHER_DEBITS" },
                    { new Guid("adc25139-e6f6-432a-879b-1f05e6bd32b8"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "EQUIPMENT" },
                    { new Guid("b0c380a7-8472-4e45-a04d-5f9920b2877a"), new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "LOAN_RECEIVED" },
                    { new Guid("b1ac6638-318d-423f-8c4c-fa3f614fd2e1"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "SUPPLIES" },
                    { new Guid("c5617a27-be6e-4848-82e8-bbf0f6548097"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "COMMUNICATION" },
                    { new Guid("c87bfe80-d970-410f-b503-5355c5a372d3"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "MAINTENANCE" },
                    { new Guid("cbb3f1ea-91cb-42e8-8daf-f68bfa520c2f"), new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "REVENUE" },
                    { new Guid("d371511b-f8d7-4468-8e9f-d540e7c0d7b6"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "FEES" },
                    { new Guid("f38043ac-9294-4909-af22-e260f9e871bb"), new Guid("880fd242-952b-4b4a-82ca-9a32a2e3866c"), "EDUCATION" },
                    { new Guid("f841be5a-9923-419b-87a2-c3654e02620a"), new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "OTHER_INCOME" },
                    { new Guid("fdaacfad-b3c9-4c5a-9c3b-3ce9667fabed"), new Guid("a7964402-ab2b-4a56-9670-72f723364f80"), "INVESTMENTS" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_category_entry_type_id",
                table: "category",
                column: "entry_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_entry_category_id",
                table: "entry",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_entry_entry_type_id",
                table: "entry",
                column: "entry_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entry");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "entry_type");
        }
    }
}
