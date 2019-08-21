using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Infrastructure.Migrations
{
    public partial class V2019_08_19_1542 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "banking");

            migrationBuilder.CreateSequence(
                name: "accountSeq",
                schema: "banking",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "IntegrationEventLogs",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    EventTypeName = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TimesSent = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLogs", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "AccountStatus",
                schema: "banking",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "banking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 32, nullable: false),
                    LastName = table.Column<string>(maxLength: 32, nullable: false),
                    BirthDay = table.Column<DateTime>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "banking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CustomerId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    AccountStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountStatus_AccountStatusId",
                        column: x => x.AccountStatusId,
                        principalSchema: "banking",
                        principalTable: "AccountStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "banking",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Credits",
                schema: "banking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<long>(nullable: false),
                    Amount_Value = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credits_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "banking",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Debits",
                schema: "banking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<long>(nullable: false),
                    Amount_Value = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Debits_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "banking",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "banking",
                table: "AccountStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Opened" });

            migrationBuilder.InsertData(
                schema: "banking",
                table: "AccountStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Locked" });

            migrationBuilder.InsertData(
                schema: "banking",
                table: "AccountStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Closed" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountStatusId",
                schema: "banking",
                table: "Accounts",
                column: "AccountStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CustomerId",
                schema: "banking",
                table: "Accounts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_AccountId",
                schema: "banking",
                table: "Credits",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Debits_AccountId",
                schema: "banking",
                table: "Debits",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventLogs");

            migrationBuilder.DropTable(
                name: "Credits",
                schema: "banking");

            migrationBuilder.DropTable(
                name: "Debits",
                schema: "banking");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "banking");

            migrationBuilder.DropTable(
                name: "AccountStatus",
                schema: "banking");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "banking");

            migrationBuilder.DropSequence(
                name: "accountSeq",
                schema: "banking");
        }
    }
}
