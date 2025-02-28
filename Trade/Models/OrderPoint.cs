using System;
using System.Collections.Generic;

namespace Trade;

public partial class OrderPoint
{
    public int PointId { get; set; }

    public int Postal { get; set; }

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
