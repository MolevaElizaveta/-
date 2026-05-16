using Moleva.Services;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WarehouseData.Models;

namespace Moleva
{
	public partial class MainWindow : Window
	{
		private InMemoryDatabase db;
		private Organization? selectedOrganization;
		private Warehouse? selectedWarehouse;
		private Product? selectedProduct;

		public MainWindow()
		{
			InitializeComponent();
			db = new InMemoryDatabase();
			RefreshOrganizationsList();
		}

		private string? ShowInputDialog(string prompt, string title, string defaultValue = "")
		{
			var dialog = new InputDialog(prompt, title, defaultValue);
			return dialog.ShowDialog() == true ? dialog.Answer : null;
		}

		private void RefreshOrganizationsList()
		{
			OrganizationsList.ItemsSource = null;
			OrganizationsList.ItemsSource = db.Organizations;
		}

		private void OrganizationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			selectedOrganization = OrganizationsList.SelectedItem as Organization;
			if (selectedOrganization != null)
			{
				SelectedOrgText.Text = $"Выбрана: {selectedOrganization.OrgName}";
				RefreshWarehousesList();
			}
			else
			{
				SelectedOrgText.Text = "Выберите организацию";
				WarehousesList.ItemsSource = null;
				SelectedWhText.Text = "Выберите склад";
				ProductsGrid.ItemsSource = null;
			}
		}

		private void RefreshWarehousesList()
		{
			if (selectedOrganization == null) return;
			WarehousesList.ItemsSource = null;
			var warehouses = db.Warehouses.Where(w => w.OrgId == selectedOrganization.OrgId).ToList();
			WarehousesList.ItemsSource = warehouses;
		}

		private void WarehousesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			selectedWarehouse = WarehousesList.SelectedItem as Warehouse;
			if (selectedWarehouse != null)
			{
				SelectedWhText.Text = $"Выбран: {selectedWarehouse.WhName}";
				RefreshProductsList();
			}
			else
			{
				SelectedWhText.Text = "Выберите склад";
				ProductsGrid.ItemsSource = null;
			}
		}

		private void RefreshProductsList()
		{
			if (selectedWarehouse == null) return;
			ProductsGrid.ItemsSource = null;
			ProductsGrid.ItemsSource = selectedWarehouse.products;
		}

		private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			selectedProduct = ProductsGrid.SelectedItem as Product;
		}

		private void AddOrgBtn_Click(object sender, RoutedEventArgs e)
		{
			string? name = ShowInputDialog("Введите название организации:", "Новая организация");
			if (!string.IsNullOrWhiteSpace(name))
			{
				var newOrg = new Organization(name);
				db.Organizations.Add(newOrg);
				RefreshOrganizationsList();
			}
		}

		private void EditOrgBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedOrganization == null)
			{
				MessageBox.Show("Выберите организацию");
				return;
			}
			string? newName = ShowInputDialog("Введите новое название:", "Редактирование", selectedOrganization.OrgName);
			if (!string.IsNullOrWhiteSpace(newName))
			{
				selectedOrganization.OrgName = newName;
				RefreshOrganizationsList();
			}
		}

		private void DeleteOrgBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedOrganization == null)
			{
				MessageBox.Show("Выберите организацию");
				return;
			}

			string? confirm = ShowInputDialog($"Введите '{selectedOrganization.OrgName}' для подтверждения:", "Подтверждение");

			if (confirm == selectedOrganization.OrgName)
			{
				db.Organizations.Remove(selectedOrganization);
				RefreshOrganizationsList();
				selectedOrganization = null;
				WarehousesList.ItemsSource = null;
				SelectedOrgText.Text = "Выберите организацию";
				SelectedWhText.Text = "Выберите склад";
				ProductsGrid.ItemsSource = null;
			}
			else if (confirm != null)
			{
				MessageBox.Show("Название не совпадает");
			}
		}

		private void AddWhBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedOrganization == null)
			{
				MessageBox.Show("Сначала выберите организацию");
				return;
			}

			string? name = ShowInputDialog("Введите название склада:", "Новый склад");
			if (!string.IsNullOrWhiteSpace(name))
			{
				string? address = ShowInputDialog("Введите адрес склада:", "Адрес склада") ?? "";
				var newWh = new Warehouse(name, address, selectedOrganization.OrgId);
				db.Warehouses.Add(newWh);
				selectedOrganization.warehouses.Add(newWh);
				RefreshWarehousesList();
			}
		}

		private void EditWhBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedWarehouse == null)
			{
				MessageBox.Show("Выберите склад");
				return;
			}
			string? newName = ShowInputDialog("Введите новое название:", "Редактирование", selectedWarehouse.WhName);
			if (!string.IsNullOrWhiteSpace(newName))
			{
				selectedWarehouse.WhName = newName;
				RefreshWarehousesList();
			}
		}

		private void DeleteWhBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedWarehouse == null)
			{
				MessageBox.Show("Выберите склад");
				return;
			}

			if (MessageBox.Show($"Удалить склад '{selectedWarehouse.WhName}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				db.Warehouses.Remove(selectedWarehouse);
				selectedOrganization?.warehouses.Remove(selectedWarehouse);
				RefreshWarehousesList();
				selectedWarehouse = null;
				ProductsGrid.ItemsSource = null;
				SelectedWhText.Text = "Выберите склад";
			}
		}

		private void AddProductBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedWarehouse == null)
			{
				MessageBox.Show("Сначала выберите склад");
				return;
			}

			var dialog = new ProductDialog(db, selectedWarehouse);
			dialog.Owner = this;
			if (dialog.ShowDialog() == true && dialog.NewProduct != null)
			{
				selectedWarehouse.products.Add(dialog.NewProduct);
				RefreshProductsList();
			}
		}

		private void EditProductBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedProduct == null)
			{
				MessageBox.Show("Выберите товар");
				return;
			}
			if (selectedWarehouse == null) return;

			var dialog = new ProductDialog(db, selectedWarehouse, selectedProduct);
			dialog.Owner = this;
			if (dialog.ShowDialog() == true)
			{
				RefreshProductsList();
			}
		}

		private void DeleteProductBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedProduct == null)
			{
				MessageBox.Show("Выберите товар");
				return;
			}
			if (selectedWarehouse == null) return;

			if (MessageBox.Show($"Удалить товар '{selectedProduct.Name}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				selectedWarehouse.products.Remove(selectedProduct);
				RefreshProductsList();
				selectedProduct = null;
			}
		}

		private void ImportBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedWarehouse == null)
			{
				MessageBox.Show("Сначала выберите склад");
				return;
			}

			var dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.Filter = "CSV files (*.csv)|*.csv";
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var lines = System.IO.File.ReadAllLines(dialog.FileName);
					int added = 0;
					foreach (var line in lines.Skip(1))
					{
						var parts = line.Split(',');
						if (parts.Length >= 2)
						{
							var newProduct = new Product
							{
								Article = parts[0].Trim(),
								Name = parts[1].Trim(),
								Price = parts.Length > 2 && decimal.TryParse(parts[2], out decimal p) ? p : 0,
								StockQuantity = parts.Length > 3 && int.TryParse(parts[3], out int q) ? q : 0,
								Unit = "шт"
							};
							selectedWarehouse.products.Add(newProduct);
							added++;
						}
					}
					RefreshProductsList();
					MessageBox.Show($"Импортировано {added} товаров");
				}
				catch (System.Exception ex)
				{
					MessageBox.Show($"Ошибка: {ex.Message}");
				}
			}
		}

		private void InvoiceBtn_Click(object sender, RoutedEventArgs e)
		{
			if (selectedWarehouse == null)
			{
				MessageBox.Show("Сначала выберите склад");
				return;
			}

			var dialog = new InvoiceDialog(selectedWarehouse.products.ToList());
			dialog.Owner = this;
			if (dialog.ShowDialog() == true)
			{
				RefreshProductsList();
				MessageBox.Show("Остатки обновлены");
			}
		}
	}

	// Диалог ввода текста
	public class InputDialog : Window
	{
		public string? Answer { get; private set; }
		private TextBox textBox;

		public InputDialog(string prompt, string title = "Ввод", string defaultValue = "")
		{
			Title = title;
			Width = 350;
			Height = 150;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;

			var grid = new Grid();
			grid.Margin = new Thickness(10);
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			grid.Children.Add(new TextBlock { Text = prompt, Margin = new Thickness(0, 0, 0, 10) });
			Grid.SetRow(grid.Children[^1], 0);

			textBox = new TextBox { Text = defaultValue, Margin = new Thickness(0, 0, 0, 10) };
			grid.Children.Add(textBox);
			Grid.SetRow(textBox, 1);

			var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
			var okBtn = new Button { Content = "OK", Width = 80, Margin = new Thickness(0, 0, 10, 0) };
			okBtn.Click += (s, e) => { Answer = textBox.Text; DialogResult = true; Close(); };
			var cancelBtn = new Button { Content = "Отмена", Width = 80 };
			cancelBtn.Click += (s, e) => { DialogResult = false; Close(); };
			panel.Children.Add(okBtn);
			panel.Children.Add(cancelBtn);
			grid.Children.Add(panel);
			Grid.SetRow(panel, 2);

			Content = grid;
			Loaded += (s, e) => textBox.Focus();
		}
	}

	// Диалог товара
	public class ProductDialog : Window
	{
		private InMemoryDatabase db;
		private Product? editingProduct;
		private TextBox articleBox, nameBox, priceBox, stockBox, discountBox;
		private ComboBox categoryBox, manufacturerBox, supplierBox;
		public Product? NewProduct { get; set; }

		public ProductDialog(InMemoryDatabase database, Warehouse warehouse, Product? product = null)
		{
			db = database;
			editingProduct = product;

			Title = product == null ? "Новый товар" : "Редактирование товара";
			Width = 450;
			Height = 550;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;

			var grid = new Grid();
			grid.Margin = new Thickness(15);
			for (int i = 0; i < 20; i++)
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			int row = 0;

			grid.Children.Add(new TextBlock { Text = "Артикул:", FontWeight = FontWeights.Bold });
			Grid.SetRow(grid.Children[^1], row++);
			articleBox = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30 };
			grid.Children.Add(articleBox);
			Grid.SetRow(articleBox, row++);

			grid.Children.Add(new TextBlock { Text = "Наименование:", FontWeight = FontWeights.Bold });
			Grid.SetRow(grid.Children[^1], row++);
			nameBox = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30 };
			grid.Children.Add(nameBox);
			Grid.SetRow(nameBox, row++);

			grid.Children.Add(new TextBlock { Text = "Цена:", FontWeight = FontWeights.Bold });
			Grid.SetRow(grid.Children[^1], row++);
			priceBox = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30 };
			grid.Children.Add(priceBox);
			Grid.SetRow(priceBox, row++);

			grid.Children.Add(new TextBlock { Text = "Остаток:" });
			Grid.SetRow(grid.Children[^1], row++);
			stockBox = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30, Text = "0" };
			grid.Children.Add(stockBox);
			Grid.SetRow(stockBox, row++);

			grid.Children.Add(new TextBlock { Text = "Скидка %:" });
			Grid.SetRow(grid.Children[^1], row++);
			discountBox = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30, Text = "0" };
			grid.Children.Add(discountBox);
			Grid.SetRow(discountBox, row++);

			grid.Children.Add(new TextBlock { Text = "Категория:" });
			Grid.SetRow(grid.Children[^1], row++);
			categoryBox = new ComboBox { Margin = new Thickness(0, 0, 0, 10), Height = 30, DisplayMemberPath = "Name" };
			categoryBox.ItemsSource = db.Categories;
			grid.Children.Add(categoryBox);
			Grid.SetRow(categoryBox, row++);

			grid.Children.Add(new TextBlock { Text = "Производитель:" });
			Grid.SetRow(grid.Children[^1], row++);
			manufacturerBox = new ComboBox { Margin = new Thickness(0, 0, 0, 10), Height = 30, DisplayMemberPath = "Name" };
			manufacturerBox.ItemsSource = db.Manufacturers;
			grid.Children.Add(manufacturerBox);
			Grid.SetRow(manufacturerBox, row++);

			grid.Children.Add(new TextBlock { Text = "Поставщик:" });
			Grid.SetRow(grid.Children[^1], row++);
			supplierBox = new ComboBox { Margin = new Thickness(0, 0, 0, 10), Height = 30, DisplayMemberPath = "Name" };
			supplierBox.ItemsSource = db.Suppliers;
			grid.Children.Add(supplierBox);
			Grid.SetRow(supplierBox, row++);

			var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 20, 0, 0) };
			var saveBtn = new Button { Content = "Сохранить", Width = 100, Height = 35, Margin = new Thickness(0, 0, 10, 0) };
			saveBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
			saveBtn.Foreground = System.Windows.Media.Brushes.White;
			saveBtn.Click += Save_Click;
			var cancelBtn = new Button { Content = "Отмена", Width = 100, Height = 35 };
			cancelBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
			cancelBtn.Foreground = System.Windows.Media.Brushes.White;
			cancelBtn.Click += (s, e) => { DialogResult = false; Close(); };
			panel.Children.Add(saveBtn);
			panel.Children.Add(cancelBtn);
			grid.Children.Add(panel);
			Grid.SetRow(panel, row);

			Content = grid;

			if (product != null)
			{
				articleBox.Text = product.Article;
				nameBox.Text = product.Name;
				priceBox.Text = product.Price.ToString();
				stockBox.Text = product.StockQuantity.ToString();
				discountBox.Text = product.DiscountPercent.ToString();
				categoryBox.SelectedItem = db.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
				manufacturerBox.SelectedItem = db.Manufacturers.FirstOrDefault(m => m.Id == product.ManufacturerId);
				supplierBox.SelectedItem = db.Suppliers.FirstOrDefault(s => s.Id == product.SupplierId);
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(nameBox.Text))
			{
				MessageBox.Show("Введите наименование");
				return;
			}
			if (!decimal.TryParse(priceBox.Text, out decimal price) || price <= 0)
			{
				MessageBox.Show("Введите корректную цену");
				return;
			}

			int stock = int.TryParse(stockBox.Text, out int s) ? s : 0;
			decimal discount = decimal.TryParse(discountBox.Text, out decimal d) ? d : 0;
			int categoryId = (categoryBox.SelectedItem as Category)?.Id ?? 0;
			string categoryName = (categoryBox.SelectedItem as Category)?.Name ?? "";
			int manufacturerId = (manufacturerBox.SelectedItem as Manufacturer)?.Id ?? 0;
			string manufacturerName = (manufacturerBox.SelectedItem as Manufacturer)?.Name ?? "";
			int supplierId = (supplierBox.SelectedItem as Supplier)?.Id ?? 0;
			string supplierName = (supplierBox.SelectedItem as Supplier)?.Name ?? "";

			if (editingProduct != null)
			{
				editingProduct.Article = articleBox.Text;
				editingProduct.Name = nameBox.Text;
				editingProduct.Price = price;
				editingProduct.StockQuantity = stock;
				editingProduct.DiscountPercent = discount;
				editingProduct.CategoryId = categoryId;
				editingProduct.CategoryName = categoryName;
				editingProduct.ManufacturerId = manufacturerId;
				editingProduct.ManufacturerName = manufacturerName;
				editingProduct.SupplierId = supplierId;
				editingProduct.SupplierName = supplierName;
				editingProduct.Unit = "шт";
			}
			else
			{
				NewProduct = new Product
				{
					Article = articleBox.Text,
					Name = nameBox.Text,
					Price = price,
					StockQuantity = stock,
					DiscountPercent = discount,
					CategoryId = categoryId,
					CategoryName = categoryName,
					ManufacturerId = manufacturerId,
					ManufacturerName = manufacturerName,
					SupplierId = supplierId,
					SupplierName = supplierName,
					Unit = "шт"
				};
			}
			DialogResult = true;
			Close();
		}
	}

	// Диалог накладной
	public class InvoiceDialog : Window
	{
		private List<Product> products;
		private List<int> originalQuantities;
		private ComboBox typeBox;
		private DataGrid grid;

		public InvoiceDialog(List<Product> warehouseProducts)
		{
			products = warehouseProducts.ToList();
			originalQuantities = products.Select(p => p.StockQuantity).ToList();

			Title = "Накладная";
			Width = 600;
			Height = 450;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;

			var mainGrid = new Grid();
			mainGrid.Margin = new Thickness(10);
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			typeBox = new ComboBox { Margin = new Thickness(0, 0, 0, 10), Height = 30, SelectedIndex = 0 };
			typeBox.Items.Add("Поступление (приход)");
			typeBox.Items.Add("Списание (расход)");
			mainGrid.Children.Add(typeBox);
			Grid.SetRow(typeBox, 0);

			grid = new DataGrid { AutoGenerateColumns = false, Height = 250, Margin = new Thickness(0, 0, 0, 10) };
			grid.Columns.Add(new DataGridTextColumn { Header = "Товар", Binding = new Binding("Name"), Width = 150 });
			grid.Columns.Add(new DataGridTextColumn { Header = "Текущий остаток", Binding = new Binding("StockQuantity"), Width = 100 });

			var factory = new FrameworkElementFactory(typeof(TextBox));
			factory.SetValue(TextBox.TextProperty, "0");
			var quantityColumn = new DataGridTemplateColumn { Header = "Количество", Width = 100 };
			quantityColumn.CellTemplate = new DataTemplate { VisualTree = factory };
			grid.Columns.Add(quantityColumn);

			grid.ItemsSource = products;
			mainGrid.Children.Add(grid);
			Grid.SetRow(grid, 1);

			var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
			var applyBtn = new Button { Content = "Применить", Width = 100, Margin = new Thickness(0, 0, 10, 0) };
			applyBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
			applyBtn.Foreground = System.Windows.Media.Brushes.White;
			applyBtn.Click += Apply_Click;
			var cancelBtn = new Button { Content = "Отмена", Width = 100 };
			cancelBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
			cancelBtn.Foreground = System.Windows.Media.Brushes.White;
			cancelBtn.Click += (s, e) => { DialogResult = false; Close(); };
			panel.Children.Add(applyBtn);
			panel.Children.Add(cancelBtn);
			mainGrid.Children.Add(panel);
			Grid.SetRow(panel, 2);

			Content = mainGrid;
		}

		private void Apply_Click(object sender, RoutedEventArgs e)
		{
			bool isIncome = typeBox.SelectedIndex == 0;

			for (int i = 0; i < products.Count; i++)
			{
				var row = grid.ItemContainerGenerator.ContainerFromIndex(i);
				if (row != null)
				{
					var presenter = FindVisualChild<ContentPresenter>(row);
					if (presenter != null)
					{
						var quantityBox = FindVisualChild<TextBox>(presenter);
						if (quantityBox != null && int.TryParse(quantityBox.Text, out int qty) && qty > 0)
						{
							if (isIncome)
								products[i].StockQuantity = originalQuantities[i] + qty;
							else
								products[i].StockQuantity = originalQuantities[i] - qty;
						}
					}
				}
			}
			DialogResult = true;
			Close();
		}

		private T? FindVisualChild<T>(DependencyObject parent) where T : FrameworkElement
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				if (child is T t) return t;
				var result = FindVisualChild<T>(child);
				if (result != null) return result;
			}
			return null;
		}
	}

	// Конвертер для фото
	public class PathToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string path && !string.IsNullOrEmpty(path))
			{
				try
				{
					return new BitmapImage(new Uri(path, UriKind.Relative));
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}