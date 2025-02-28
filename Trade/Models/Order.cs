using System;
using System.Collections.Generic;

namespace Trade;

public partial class Order
{
    public int OrderId { get; set; }

    public string OrderStatus { get; set; } = null!;

    public DateTime OrderDeliveryDate { get; set; }

    public int? OrderPickupPoint { get; set; }

    public virtual OrderPoint? OrderPickupPointNavigation { get; set; }

    public virtual ICollection<Product> ProductArticleNumbers { get; set; } = new List<Product>();
}
