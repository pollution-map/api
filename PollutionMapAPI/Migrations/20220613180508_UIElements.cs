using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollutionMapAPI.Migrations
{
    public partial class UIElements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatasetsProperties_UIElements_UIElementId",
                table: "DatasetsProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_DatasetsPropertiesValues_DatasetsItems_DatasetItemId",
                table: "DatasetsPropertiesValues");

            migrationBuilder.DropForeignKey(
                name: "FK_UIElements_MapUIs_MapUIId",
                table: "UIElements");

            migrationBuilder.DropTable(
                name: "MapUIs");

            migrationBuilder.DropIndex(
                name: "IX_UIElements_MapUIId",
                table: "UIElements");

            migrationBuilder.DropIndex(
                name: "IX_DatasetsProperties_DataSetId",
                table: "DatasetsProperties");

            migrationBuilder.DropIndex(
                name: "IX_DatasetsProperties_UIElementId",
                table: "DatasetsProperties");

            migrationBuilder.DropColumn(
                name: "MapUIId",
                table: "UIElements");

            migrationBuilder.DropColumn(
                name: "UIElementId",
                table: "DatasetsProperties");

            migrationBuilder.AddColumn<Guid>(
                name: "UIId",
                table: "UIElements",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UIId",
                table: "Maps",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "DatasetItemId",
                table: "DatasetsPropertiesValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DatasetPropertyUIElement",
                columns: table => new
                {
                    PropertiesId = table.Column<long>(type: "bigint", nullable: false),
                    UIElementsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetPropertyUIElement", x => new { x.PropertiesId, x.UIElementsId });
                    table.ForeignKey(
                        name: "FK_DatasetPropertyUIElement_DatasetsProperties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "DatasetsProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatasetPropertyUIElement_UIElements_UIElementsId",
                        column: x => x.UIElementsId,
                        principalTable: "UIElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UIs_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UIElements_UIId",
                table: "UIElements",
                column: "UIId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetsProperties_DataSetId_PropertyName",
                table: "DatasetsProperties",
                columns: new[] { "DataSetId", "PropertyName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetPropertyUIElement_UIElementsId",
                table: "DatasetPropertyUIElement",
                column: "UIElementsId");

            migrationBuilder.CreateIndex(
                name: "IX_UIs_MapId",
                table: "UIs",
                column: "MapId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DatasetsPropertiesValues_DatasetsItems_DatasetItemId",
                table: "DatasetsPropertiesValues",
                column: "DatasetItemId",
                principalTable: "DatasetsItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UIElements_UIs_UIId",
                table: "UIElements",
                column: "UIId",
                principalTable: "UIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatasetsPropertiesValues_DatasetsItems_DatasetItemId",
                table: "DatasetsPropertiesValues");

            migrationBuilder.DropForeignKey(
                name: "FK_UIElements_UIs_UIId",
                table: "UIElements");

            migrationBuilder.DropTable(
                name: "DatasetPropertyUIElement");

            migrationBuilder.DropTable(
                name: "UIs");

            migrationBuilder.DropIndex(
                name: "IX_UIElements_UIId",
                table: "UIElements");

            migrationBuilder.DropIndex(
                name: "IX_DatasetsProperties_DataSetId_PropertyName",
                table: "DatasetsProperties");

            migrationBuilder.DropColumn(
                name: "UIId",
                table: "UIElements");

            migrationBuilder.DropColumn(
                name: "UIId",
                table: "Maps");

            migrationBuilder.AddColumn<Guid>(
                name: "MapUIId",
                table: "UIElements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DatasetItemId",
                table: "DatasetsPropertiesValues",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "UIElementId",
                table: "DatasetsProperties",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MapUIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapUIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapUIs_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UIElements_MapUIId",
                table: "UIElements",
                column: "MapUIId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetsProperties_DataSetId",
                table: "DatasetsProperties",
                column: "DataSetId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetsProperties_UIElementId",
                table: "DatasetsProperties",
                column: "UIElementId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUIs_MapId",
                table: "MapUIs",
                column: "MapId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatasetsProperties_UIElements_UIElementId",
                table: "DatasetsProperties",
                column: "UIElementId",
                principalTable: "UIElements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DatasetsPropertiesValues_DatasetsItems_DatasetItemId",
                table: "DatasetsPropertiesValues",
                column: "DatasetItemId",
                principalTable: "DatasetsItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UIElements_MapUIs_MapUIId",
                table: "UIElements",
                column: "MapUIId",
                principalTable: "MapUIs",
                principalColumn: "Id");
        }
    }
}
