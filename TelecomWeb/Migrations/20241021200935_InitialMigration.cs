using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffPosition",
                columns: table => new
                {
                    PositionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StaffPos__60BB9A59D4D3AD5C", x => x.PositionID);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    SubscriberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HomeAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PassportData = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrib__7DFEB634B3799F70", x => x.SubscriberID);
                });

            migrationBuilder.CreateTable(
                name: "TariffPlans",
                columns: table => new
                {
                    TariffPlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TariffName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubscriptionFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LocalCallRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LongDistanceCallRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    InternationalCallRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsPerSecond = table.Column<bool>(type: "bit", nullable: false),
                    SmsRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MmsRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DataRatePerMB = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TariffPl__29A9282A1C614268", x => x.TariffPlanID);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false),
                    Education = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Staff__96D4AAF7835BD125", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK__Staff__PositionI__093F5D4E",
                        column: x => x.PositionID,
                        principalTable: "StaffPosition",
                        principalColumn: "PositionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ContractID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriberID = table.Column<int>(type: "int", nullable: false),
                    TariffPlanID = table.Column<int>(type: "int", nullable: false),
                    ContractDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Contract__C90D34095C346A02", x => x.ContractID);
                    table.ForeignKey(
                        name: "FK__Contracts__Staff__11D4A34F",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Contracts__Subsc__0FEC5ADD",
                        column: x => x.SubscriberID,
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Contracts__Tarif__10E07F16",
                        column: x => x.TariffPlanID,
                        principalTable: "TariffPlans",
                        principalColumn: "TariffPlanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calls",
                columns: table => new
                {
                    CallID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractID = table.Column<int>(type: "int", nullable: false),
                    CallDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CallDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Calls__5180CF8A3D2D2ED2", x => x.CallID);
                    table.ForeignKey(
                        name: "FK__Calls__ContractI__14B10FFA",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ContractID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InternetUsage",
                columns: table => new
                {
                    UsageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractID = table.Column<int>(type: "int", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DataSentMB = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DataReceivedMB = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Internet__29B197C0F4400AED", x => x.UsageID);
                    table.ForeignKey(
                        name: "FK__InternetU__Contr__1A69E950",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ContractID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractID = table.Column<int>(type: "int", nullable: false),
                    MessageDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsMMS = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Messages__C87C037C0167DB96", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK__Messages__Contra__178D7CA5",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ContractID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calls_ContractID",
                table: "Calls",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_StaffID",
                table: "Contracts",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SubscriberID",
                table: "Contracts",
                column: "SubscriberID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_TariffPlanID",
                table: "Contracts",
                column: "TariffPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_InternetUsage_ContractID",
                table: "InternetUsage",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ContractID",
                table: "Messages",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionID",
                table: "Staff",
                column: "PositionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calls");

            migrationBuilder.DropTable(
                name: "InternetUsage");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "TariffPlans");

            migrationBuilder.DropTable(
                name: "StaffPosition");
        }
    }
}
