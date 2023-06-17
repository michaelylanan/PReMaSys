using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class Initialreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "prms");

            migrationBuilder.CreateTable(
                name: "AccLogin",
                schema: "prms",
                columns: table => new
                {
                    accID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccLogin", x => x.accID);
                });

            migrationBuilder.CreateTable(
                name: "AccLoginSE",
                schema: "prms",
                columns: table => new
                {
                    accID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccLoginSE", x => x.accID);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "prms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainAccount",
                schema: "prms",
                columns: table => new
                {
                    AdminInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyLogo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessSize = table.Column<int>(type: "int", nullable: false),
                    NatureOfBusiness = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyBday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminPicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminLastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminFirstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainAccount", x => x.AdminInfoId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "prms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesPerformances",
                schema: "prms",
                columns: table => new
                {
                    SalesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitsSold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostPricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SellingPricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesVolume = table.Column<int>(type: "int", nullable: false),
                    ConversionR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AverageDealSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerAcquisition = table.Column<int>(type: "int", nullable: false),
                    CustomerRetentionR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPerformances", x => x.SalesID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "prms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pic = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAffiliation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NatureOfBusiness = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyBday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_userId",
                        column: x => x.userId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "prms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "prms",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesForecasts",
                schema: "prms",
                columns: table => new
                {
                    ForecastID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SPID = table.Column<int>(type: "int", nullable: false),
                    SalesPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WeeklyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QuarterlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YearlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesForecasts", x => x.ForecastID);
                    table.ForeignKey(
                        name: "FK_SalesForecasts_SalesPerformances_SPID",
                        column: x => x.SPID,
                        principalSchema: "prms",
                        principalTable: "SalesPerformances",
                        principalColumn: "SalesID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminRoles",
                schema: "prms",
                columns: table => new
                {
                    AdminRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    AdminPicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminEmployeeNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminFirstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminLastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRoles", x => x.AdminRoleId);
                    table.ForeignKey(
                        name: "FK_AdminRoles_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                schema: "prms",
                columns: table => new
                {
                    RewardsInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RewardName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RewardCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PointsCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    inventory = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.RewardsInformationId);
                    table.ForeignKey(
                        name: "FK_Rewards_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesEmployeeRecords",
                schema: "prms",
                columns: table => new
                {
                    SEmployeeRecordsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmployeeNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeFirstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    EmployeeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeBirthdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeePoints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesEmployeeRecords", x => x.SEmployeeRecordsID);
                    table.ForeignKey(
                        name: "FK_SalesEmployeeRecords_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SERecord",
                schema: "prms",
                columns: table => new
                {
                    SEmployeeRecordsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SERIdId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmployeeNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeFirstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeBirthdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeePoints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SERecord", x => x.SEmployeeRecordsID);
                    table.ForeignKey(
                        name: "FK_SERecord_Users_SERIdId",
                        column: x => x.SERIdId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "prms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "prms",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "prms",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "prms",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "prms",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddToCart",
                schema: "prms",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RewardsInformationId = table.Column<int>(type: "int", nullable: false),
                    RewardImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RewardName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    RewardDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RewardPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddToCart", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_AddToCart_Rewards_RewardsInformationId",
                        column: x => x.RewardsInformationId,
                        principalSchema: "prms",
                        principalTable: "Rewards",
                        principalColumn: "RewardsInformationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddToCart_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Purchase",
                schema: "prms",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AddToCartCartId = table.Column<int>(type: "int", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RewardImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RewardName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RewardPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stat = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchase", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK_Purchase_AddToCart_AddToCartCartId",
                        column: x => x.AddToCartCartId,
                        principalSchema: "prms",
                        principalTable: "AddToCart",
                        principalColumn: "CartId");
                    table.ForeignKey(
                        name: "FK_Purchase_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "prms",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddToCart_ApplicationUserId",
                schema: "prms",
                table: "AddToCart",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AddToCart_RewardsInformationId",
                schema: "prms",
                table: "AddToCart",
                column: "RewardsInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminRoles_ApplicationUserId",
                schema: "prms",
                table: "AdminRoles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_AddToCartCartId",
                schema: "prms",
                table: "Purchase",
                column: "AddToCartCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_ApplicationUserId",
                schema: "prms",
                table: "Purchase",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_ApplicationUserId",
                schema: "prms",
                table: "Rewards",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "prms",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "prms",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SalesEmployeeRecords_ApplicationUserId",
                schema: "prms",
                table: "SalesEmployeeRecords",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesForecasts_SPID",
                schema: "prms",
                table: "SalesForecasts",
                column: "SPID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SERecord_SERIdId",
                schema: "prms",
                table: "SERecord",
                column: "SERIdId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "prms",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "prms",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "prms",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "prms",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_userId",
                schema: "prms",
                table: "Users",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "prms",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccLogin",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "AccLoginSE",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "AdminRoles",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "DomainAccount",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "Purchase",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SalesEmployeeRecords",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SalesForecasts",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SERecord",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "AddToCart",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SalesPerformances",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "Rewards",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "prms");
        }
    }
}
