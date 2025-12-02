using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Bookingextraasset
{
    public int BookingId { get; set; }

    public int ExtraAssetId { get; set; }

    public decimal ExtraAssetPrice { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Extraasset ExtraAsset { get; set; } = null!;
}
