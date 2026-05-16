using System.Collections.ObjectModel;
using WarehouseData.Models;

namespace Moleva.Services
{
	public class InMemoryDatabase
	{
		public ObservableCollection<Organization> Organizations { get; set; }
		public ObservableCollection<Warehouse> Warehouses { get; set; }
		public ObservableCollection<Product> Products { get; set; }
		public ObservableCollection<Category> Categories { get; set; }
		public ObservableCollection<Manufacturer> Manufacturers { get; set; }
		public ObservableCollection<Supplier> Suppliers { get; set; }

		public InMemoryDatabase()
		{
			Organizations = new ObservableCollection<Organization>();
			Warehouses = new ObservableCollection<Warehouse>();
			Products = new ObservableCollection<Product>();
			Categories = new ObservableCollection<Category>();
			Manufacturers = new ObservableCollection<Manufacturer>();
			Suppliers = new ObservableCollection<Supplier>();

			LoadTestData();
		}

		private void LoadTestData()
		{
			// Категории
			Categories.Add(new Category { Id = 1, Name = "Электроника" });
			Categories.Add(new Category { Id = 2, Name = "Мебель" });
			Categories.Add(new Category { Id = 3, Name = "Одежда" });

			// Производители
			Manufacturers.Add(new Manufacturer { Id = 1, Name = "Samsung" });
			Manufacturers.Add(new Manufacturer { Id = 2, Name = "IKEA" });
			Manufacturers.Add(new Manufacturer { Id = 3, Name = "Nike" });

			// Поставщики
			Suppliers.Add(new Supplier { Id = 1, Name = "ООО ТехноПоставка" });
			Suppliers.Add(new Supplier { Id = 2, Name = "МебельТрейд" });
			Suppliers.Add(new Supplier { Id = 3, Name = "СпортИмпорт" });

			// Организации
			var org1 = new Organization("ООО Рога и Копыта");
			var org2 = new Organization("ЗАО ТехноСервис");
			Organizations.Add(org1);
			Organizations.Add(org2);

			// Склады
			var wh1 = new Warehouse("Основной склад", "Москва, ул. Ленина 1", org1.OrgId);
			var wh2 = new Warehouse("Резервный склад", "Москва, ул. Пушкина 2", org1.OrgId);
			Warehouses.Add(wh1);
			Warehouses.Add(wh2);
			org1.warehouses.Add(wh1);
			org1.warehouses.Add(wh2);

			// Товар 1: Телевизор
			var product1 = new Product
			{
				Article = "ART001",
				Name = "Телевизор Samsung",
				Unit = "шт",
				Price = 50000,
				CategoryId = 1,
				CategoryName = "Электроника",
				ManufacturerId = 1,
				ManufacturerName = "Samsung",
				SupplierId = 1,
				SupplierName = "ООО ТехноПоставка",
				StockQuantity = 10,
				DiscountPercent = 5,
				PhotoPath = "/Resources/tv.jpg"
			};

			// Товар 2: Стул
			var product2 = new Product
			{
				Article = "ART002",
				Name = "Стул IKEA",
				Unit = "шт",
				Price = 3000,
				CategoryId = 2,
				CategoryName = "Мебель",
				ManufacturerId = 2,
				ManufacturerName = "IKEA",
				SupplierId = 2,
				SupplierName = "МебельТрейд",
				StockQuantity = 50,
				DiscountPercent = 0,
				PhotoPath = "/Resources/chair.jpg"
			};

			// Товар 3: Кроссовки
			var product3 = new Product
			{
				Article = "ART003",
				Name = "Кроссовки Nike",
				Unit = "пара",
				Price = 8000,
				CategoryId = 3,
				CategoryName = "Одежда",
				ManufacturerId = 3,
				ManufacturerName = "Nike",
				SupplierId = 3,
				SupplierName = "СпортИмпорт",
				StockQuantity = 25,
				DiscountPercent = 10,
				PhotoPath = "/Resources/nike.jpg"
			};

			Products.Add(product1);
			Products.Add(product2);
			Products.Add(product3);
			wh1.products.Add(product1);
			wh1.products.Add(product2);
			wh1.products.Add(product3);
		}
	}
}