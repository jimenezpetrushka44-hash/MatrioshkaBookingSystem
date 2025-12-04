using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Extraasset
{
    public int ExtraAssetId { get; set; }

    public string ExtraAssetName { get; set; } = null!;
    public decimal AssetPrice { get; set; }

    public string ExtraAssetStatus { get; set; } = null!;

    public virtual ICollection<Bookingextraasset> Bookingextraassets { get; set; } = new List<Bookingextraasset>();
}
