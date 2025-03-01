using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Логика взаимодействия для RedactProduct.xaml
    /// </summary>
    public partial class RedactProduct : Window
    {
        public static TradeContext Trade = new();
        public RedactProduct()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите изображение",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    using (var stream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        if (bitmap.PixelHeight > 200 || bitmap.PixelWidth > 300)
                        {
                            MessageBox.Show("Размер изображения должен быть не больше 300x200", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        SelectedImage.Source = bitmap;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveInfo_Click(object sender, RoutedEventArgs e)
        {
            bool isEditMode = !Article.IsEnabled;
            if (!Check(isEditMode))
                return;
            if (isEditMode)
                Edit(Trade.Products.First(p => p.ProductArticleNumber == Article.Text), isEditMode);
            else
                Edit(new Product(), isEditMode);
            this.Close();
        }

        private bool Check(bool isEdit)
        {
            if (!ValidateArticle()) return false;

            if (!isEdit && !ValidateUniqueArticle()) return false;

            if (!ValidateName()) return false;
            if (!ValidateDescription()) return false;
            if (!ValidateCategory()) return false;
            if (!ValidateManufacturer()) return false;
            if (!ValidatePrice()) return false;
            if (!ValidateDiscount()) return false;
            if (!ValidateQuantity()) return false;

            return true;
        }

        private bool ValidateArticle()
        {
            if (string.IsNullOrEmpty(Article.Text))
            {
                ShowErrorMessage("Вы не заполнили артикул");
                return false;
            }
            return true;
        }

        private bool ValidateUniqueArticle()
        {
            if (Trade.Products.Any(p => p.ProductArticleNumber == Article.Text))
            {
                ShowErrorMessage("Товар с таким артикулом уже существует");
                return false;
            }
            return true;
        }

        private bool ValidateName()
        {
            if (string.IsNullOrEmpty(Name.Text))
            {
                ShowErrorMessage("Вы не заполнили наименование");
                return false;
            }
            return true;
        }

        private bool ValidateDescription()
        {
            if (string.IsNullOrEmpty(Desc.Text))
            {
                ShowErrorMessage("Вы не заполнили описание");
                return false;
            }
            return true;
        }

        private bool ValidateCategory()
        {
            if (string.IsNullOrEmpty(Category.Text))
            {
                ShowErrorMessage("Вы не заполнили категорию");
                return false;
            }
            return true;
        }

        private bool ValidateManufacturer()
        {
            if (string.IsNullOrEmpty(Manufracturer.Text))
            {
                ShowErrorMessage("Вы не заполнили производителя");
                return false;
            }
            return true;
        }

        private bool ValidatePrice()
        {
            Price.Text = Price.Text.Replace('.', ',');
            if (string.IsNullOrEmpty(Price.Text))
            {
                ShowErrorMessage("Вы не заполнили цену");
                return false;
            }

            if (!decimal.TryParse(Price.Text, out decimal price) || price < 1)
            {
                ShowErrorMessage("Цена должна быть числом больше нуля");
                return false;
            }

            return true;
        }

        private bool ValidateDiscount()
        {
            if (string.IsNullOrEmpty(Discount.Text))
            {
                Discount.Text = "0";
            }

            if (!byte.TryParse(Discount.Text, out byte discount) || discount < 0 || discount > 50)
            {
                ShowErrorMessage("Скидка должна быть числом от 0 до 50");
                return false;
            }

            return true;
        }

        private bool ValidateQuantity()
        {
            if (string.IsNullOrEmpty(Quantity.Text))
            {
                ShowErrorMessage("Вы не заполнили количество товара");
                return false;
            }

            if (!int.TryParse(Quantity.Text, out int quantity) || quantity < 0)
            {
                ShowErrorMessage("Количество товара должно быть положительным числом");
                return false;
            }

            return true;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Edit(Product p, bool isEdit)
        {
            if (!isEdit)
                p.ProductArticleNumber = Article.Text;
            p.ProductName = Name.Text;
            p.ProductDescription = Desc.Text;
            p.ProductCategory = Category.Text;
            p.ProductManufacturer = Manufracturer.Text;
            p.ProductCost = Convert.ToDecimal(Price.Text);
            p.ProductDiscountAmount = Convert.ToByte(Discount.Text);
            p.ProductQuantityInStock = Convert.ToInt32(Quantity.Text);
            p.ProductStatus = "0";
            if (SelectedImage == null || SelectedImage.Source == null)
                SelectedImage.Source = PlaceholderImage();
            p.ProductPhoto = ImageToByte(SelectedImage);

            if (!isEdit)
                Trade.Products.Add(p);
            else
                Trade.Products.Update(p);
            Trade.SaveChanges();
        }
        private BitmapImage PlaceholderImage()
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "picture.png");
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
        private byte[] ImageToByte(Image image)
        {
            var bitmapsource = image.Source as BitmapSource;
            if (bitmapsource == null)
                return null;
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapsource));
                encoder.Save(stream);

                return stream.ToArray();
            }

        }
    }
}
