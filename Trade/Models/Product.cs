using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Trade;

public partial class Product
{
    public string ProductArticleNumber { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public string ProductCategory { get; set; } = null!;

    public byte[] ProductPhoto { get; set; } = null!;

    public BitmapImage ProductPhotoImage => ProductPhotoInImage();

    public BitmapImage ProductPhotoInImage()
    {
        if (ProductPhoto == null || ProductPhoto.Length == 0)
            return null;
        try
        {
            using (var stream = new MemoryStream(ProductPhoto))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
        catch
        {
            return null;
        }
    }


    public string ProductManufacturer { get; set; } = null!;

    public decimal ProductCost { get; set; }
    public byte? ProductDiscountAmount { get; set; }
    public decimal? ProductCostDiscounting => ProductCost - (ProductCost * ((decimal)ProductDiscountAmount / 100));
    public bool ProductOnDiscount => ProductDiscountAmount > 0;
    public int ProductQuantityInStock { get; set; }
    public bool ProductAvailable => ProductQuantityInStock > 0;

    public string ProductStatus { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
