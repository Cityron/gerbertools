using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GerberBackend.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AngleChamfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AngleChamfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoardThickness",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardThickness", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContourMachinings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContourMachinings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataNumberings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataNumberings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrillFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrillFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EdgeConnectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdgeConnectors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoilThicknesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoilThicknesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GerberFileBinaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<byte[]>(type: "bytea", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GerberFileBinaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarkingColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkingColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarkingSides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkingSides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaskColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaskColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaskSides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaskSides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaskTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MinimalConductors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinimalConductors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    SecondName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Identity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderGerber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Size_X = table.Column<int>(type: "integer", nullable: true),
                    Size_Y = table.Column<int>(type: "integer", nullable: true),
                    Quantity_Value = table.Column<int>(type: "integer", nullable: true),
                    Count_Value = table.Column<int>(type: "integer", nullable: true),
                    BuildTime_Value = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status_Value = table.Column<string>(type: "text", nullable: true),
                    Order_Value = table.Column<string>(type: "text", nullable: true),
                    Price_Value = table.Column<int>(type: "integer", nullable: true),
                    BoardWindow_Value = table.Column<int>(type: "integer", nullable: true),
                    Lamellas_Value = table.Column<int>(type: "integer", nullable: true),
                    ConnectorsCount_Value = table.Column<int>(type: "integer", nullable: true),
                    MaskColorId = table.Column<int>(type: "integer", nullable: false),
                    MarkingColorId = table.Column<int>(type: "integer", nullable: false),
                    DataNumberingId = table.Column<int>(type: "integer", nullable: false),
                    ContourMachiningId = table.Column<int>(type: "integer", nullable: false),
                    BaseMaterialId = table.Column<int>(type: "integer", nullable: false),
                    LayerId = table.Column<int>(type: "integer", nullable: false),
                    BoardThicknessId = table.Column<int>(type: "integer", nullable: false),
                    DrillFileId = table.Column<int>(type: "integer", nullable: false),
                    GerberFileId = table.Column<int>(type: "integer", nullable: false),
                    MarkingSideId = table.Column<int>(type: "integer", nullable: false),
                    MaskTypeId = table.Column<int>(type: "integer", nullable: false),
                    MaskSideId = table.Column<int>(type: "integer", nullable: false),
                    ViasId = table.Column<int>(type: "integer", nullable: false),
                    MainSitesId = table.Column<int>(type: "integer", nullable: false),
                    EdgeConnectorId = table.Column<int>(type: "integer", nullable: false),
                    MinimalConductorId = table.Column<int>(type: "integer", nullable: false),
                    AngleChamferId = table.Column<int>(type: "integer", nullable: false),
                    FoilThicknessId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderGerber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderGerber_AngleChamfers_AngleChamferId",
                        column: x => x.AngleChamferId,
                        principalTable: "AngleChamfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_BaseMaterials_BaseMaterialId",
                        column: x => x.BaseMaterialId,
                        principalTable: "BaseMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_BoardThickness_BoardThicknessId",
                        column: x => x.BoardThicknessId,
                        principalTable: "BoardThickness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_ContourMachinings_ContourMachiningId",
                        column: x => x.ContourMachiningId,
                        principalTable: "ContourMachinings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_DataNumberings_DataNumberingId",
                        column: x => x.DataNumberingId,
                        principalTable: "DataNumberings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_DrillFiles_DrillFileId",
                        column: x => x.DrillFileId,
                        principalTable: "DrillFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_EdgeConnectors_EdgeConnectorId",
                        column: x => x.EdgeConnectorId,
                        principalTable: "EdgeConnectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_FoilThicknesses_FoilThicknessId",
                        column: x => x.FoilThicknessId,
                        principalTable: "FoilThicknesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_GerberFileBinaries_GerberFileId",
                        column: x => x.GerberFileId,
                        principalTable: "GerberFileBinaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_Layers_LayerId",
                        column: x => x.LayerId,
                        principalTable: "Layers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_MainSites_MainSitesId",
                        column: x => x.MainSitesId,
                        principalTable: "MainSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_MarkingColors_MarkingColorId",
                        column: x => x.MarkingColorId,
                        principalTable: "MarkingColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_MarkingSides_MarkingSideId",
                        column: x => x.MarkingSideId,
                        principalTable: "MarkingSides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_MaskColors_MaskColorId",
                        column: x => x.MaskColorId,
                        principalTable: "MaskColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_MaskSides_MaskSideId",
                        column: x => x.MaskSideId,
                        principalTable: "MaskSides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_MaskTypes_MaskTypeId",
                        column: x => x.MaskTypeId,
                        principalTable: "MaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_MinimalConductors_MinimalConductorId",
                        column: x => x.MinimalConductorId,
                        principalTable: "MinimalConductors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGerber_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderGerber_Vias_ViasId",
                        column: x => x.ViasId,
                        principalTable: "Vias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_AngleChamferId",
                table: "OrderGerber",
                column: "AngleChamferId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_BaseMaterialId",
                table: "OrderGerber",
                column: "BaseMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_BoardThicknessId",
                table: "OrderGerber",
                column: "BoardThicknessId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_ContourMachiningId",
                table: "OrderGerber",
                column: "ContourMachiningId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_DataNumberingId",
                table: "OrderGerber",
                column: "DataNumberingId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_DrillFileId",
                table: "OrderGerber",
                column: "DrillFileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_EdgeConnectorId",
                table: "OrderGerber",
                column: "EdgeConnectorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_FoilThicknessId",
                table: "OrderGerber",
                column: "FoilThicknessId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_GerberFileId",
                table: "OrderGerber",
                column: "GerberFileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_LayerId",
                table: "OrderGerber",
                column: "LayerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_MainSitesId",
                table: "OrderGerber",
                column: "MainSitesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_MarkingColorId",
                table: "OrderGerber",
                column: "MarkingColorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_MarkingSideId",
                table: "OrderGerber",
                column: "MarkingSideId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_MaskColorId",
                table: "OrderGerber",
                column: "MaskColorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_MaskSideId",
                table: "OrderGerber",
                column: "MaskSideId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_MaskTypeId",
                table: "OrderGerber",
                column: "MaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_MinimalConductorId",
                table: "OrderGerber",
                column: "MinimalConductorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_UserId",
                table: "OrderGerber",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGerber_ViasId",
                table: "OrderGerber",
                column: "ViasId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "OrderGerber");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AngleChamfers");

            migrationBuilder.DropTable(
                name: "BaseMaterials");

            migrationBuilder.DropTable(
                name: "BoardThickness");

            migrationBuilder.DropTable(
                name: "ContourMachinings");

            migrationBuilder.DropTable(
                name: "DataNumberings");

            migrationBuilder.DropTable(
                name: "DrillFiles");

            migrationBuilder.DropTable(
                name: "EdgeConnectors");

            migrationBuilder.DropTable(
                name: "FoilThicknesses");

            migrationBuilder.DropTable(
                name: "GerberFileBinaries");

            migrationBuilder.DropTable(
                name: "Layers");

            migrationBuilder.DropTable(
                name: "MainSites");

            migrationBuilder.DropTable(
                name: "MarkingColors");

            migrationBuilder.DropTable(
                name: "MarkingSides");

            migrationBuilder.DropTable(
                name: "MaskColors");

            migrationBuilder.DropTable(
                name: "MaskSides");

            migrationBuilder.DropTable(
                name: "MaskTypes");

            migrationBuilder.DropTable(
                name: "MinimalConductors");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vias");
        }
    }
}
