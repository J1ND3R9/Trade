using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Trade
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        public static TradeContext Trade = new();
        public ProductWindow()
        {
            InitializeComponent();
            FillManufracturers();
            FillProducts();
        }
        private void FillManufracturers()
        {
            List<string> productsManufracturers = Trade.Products.Select(e => e.ProductManufacturer).Distinct().OrderBy(p => p).ToList();
            productsManufracturers.Insert(0, "Все производители");
            FilterManufracturer.ItemsSource = productsManufracturers;
            FilterManufracturer.SelectedIndex = 0;
        }

        private void FillProducts(List<Product> products = null)
        {
            if (products == null)
                products = Trade.Products.ToList();
            Products.ItemsSource = products;
            TotalProducts.Text = Products.Items.Count.ToString();
            CountProducts.Text = Products.Items.Count.ToString();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e) => RefreshProducts();
        private void FilterCost_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshProducts();
        private void FilterManufracturer_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshProducts();

        private void RefreshProducts()
        {
            object[] objects = { FilterManufracturer, Search, FilterCost };
            if (objects.Contains(null))
            {
                return;
            }
            List<Product> products = Trade.Products.AsNoTracking().ToList();
            string? search = Search.Text.ToLower();
            string[] words = search.Split(' ');
            string manuf = FilterManufracturer.SelectedItem.ToString();
            products = products.Where(p => words.All(w => p.ProductName.ToLower().Contains(w) || p.ProductDescription.ToLower().Contains(w)) && p.ProductManufacturer.Contains(manuf == "Все производители" ? "" : manuf)).ToList();

            if (FilterCost.SelectedIndex != 0)
                products = FilterCost.SelectedIndex == 1 ? products.OrderBy(p => p.ProductCost).ToList() : products.OrderByDescending(p => p.ProductCost).ToList();

            FillProducts(products);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            RedactProduct window = new();
            Product product = Trade.Products.AsNoTracking().First(p => p.ProductArticleNumber == b.Tag.ToString());
            window.Article.Text = product.ProductArticleNumber;
            window.Article.IsEnabled = false;
            window.Name.Text = product.ProductName;
            window.Desc.Text = product.ProductDescription;
            window.Category.Text = product.ProductCategory;
            window.Manufracturer.Text = product.ProductManufacturer;
            window.Price.Text = product.ProductCost.ToString();
            window.Discount.Text = product.ProductDiscountAmount.ToString();
            window.Quantity.Text = product.ProductQuantityInStock.ToString();
            window.SelectedImage.Source = product.ProductPhotoImage;
            window.Title = "Редактирование";
            window.ShowDialog();
            RefreshProducts();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Product p = Trade.Products.First(p => p.ProductArticleNumber == b.Tag.ToString());
            if (IsOrdered(p))
            {
                MessageBox.Show("Данный продукт уже заказан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Trade.Products.Remove(p);
            Trade.SaveChanges();
            RefreshProducts();
        }
        private bool IsOrdered(Product p) => p.Orders.Intersect(Trade.Orders).Any();

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            RedactProduct window = new();
            window.Title = "Добавление";
            window.ShowDialog();
            RefreshProducts();
        }
    }
}
