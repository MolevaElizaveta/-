using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseData.Models
{
    [Table("products")]
    public class Product
    {
        [Column("article")]
        public string Article { get; set; } = null!;

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("unit")]
        public string Unit { get; set; } = null!;

        [Column("price")]
        public decimal Price { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("manufacturer_id")]
        public int ManufacturerId { get; set; }

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("discount_percent")]
        public decimal DiscountPercent { get; set; }

        [Column("stock_quantity")]
        public int StockQuantity { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("photo_path")]
        public string? PhotoPath { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = "";
        public string ManufacturerName { get; set; } = "";

        public Category Category { get; set; } = null!;
        public Manufacturer Manufacturer { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;

        public Product() { }
    }
}