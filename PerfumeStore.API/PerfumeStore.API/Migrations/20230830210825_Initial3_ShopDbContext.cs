using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfumeStore.API.Migrations
{
  /// <inheritdoc />
  public partial class Initial3_ShopDbContext : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Carts",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Carts", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "ProductCategories",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ProductCategories", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Products",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Products", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Orders",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            CartId = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Orders", x => x.Id);
            table.ForeignKey(
                      name: "FK_Orders_Carts_CartId",
                      column: x => x.CartId,
                      principalTable: "Carts",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "CartsLine",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            ProductId = table.Column<int>(type: "int", nullable: false),
            CartId = table.Column<int>(type: "int", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CartsLine", x => x.Id);
            table.ForeignKey(
                      name: "FK_CartsLine_Carts_CartId",
                      column: x => x.CartId,
                      principalTable: "Carts",
                      principalColumn: "Id");
            table.ForeignKey(
                      name: "FK_CartsLine_Products_ProductId",
                      column: x => x.ProductId,
                      principalTable: "Products",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "ProductProductCategories",
          columns: table => new
          {
            ProductId = table.Column<int>(type: "int", nullable: false),
            ProductCategoryId = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ProductProductCategories", x => new { x.ProductId, x.ProductCategoryId });
            table.ForeignKey(
                      name: "FK_ProductProductCategories_ProductCategories_ProductCategoryId",
                      column: x => x.ProductCategoryId,
                      principalTable: "ProductCategories",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_ProductProductCategories_Products_ProductId",
                      column: x => x.ProductId,
                      principalTable: "Products",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_CartsLine_CartId",
          table: "CartsLine",
          column: "CartId");

      migrationBuilder.CreateIndex(
          name: "IX_CartsLine_ProductId",
          table: "CartsLine",
          column: "ProductId");

      migrationBuilder.CreateIndex(
          name: "IX_Orders_CartId",
          table: "Orders",
          column: "CartId");

      migrationBuilder.CreateIndex(
          name: "IX_ProductProductCategories_ProductCategoryId",
          table: "ProductProductCategories",
          column: "ProductCategoryId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "CartsLine");

      migrationBuilder.DropTable(
          name: "Orders");

      migrationBuilder.DropTable(
          name: "ProductProductCategories");

      migrationBuilder.DropTable(
          name: "Carts");

      migrationBuilder.DropTable(
          name: "ProductCategories");

      migrationBuilder.DropTable(
          name: "Products");
    }
  }
}
