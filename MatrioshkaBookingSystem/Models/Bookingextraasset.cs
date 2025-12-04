using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Bookingextraasset
{
    public int BookingId { get; set; }

    public int ExtraAssetId { get; set; }

    public decimal ExtraAssetPrice { get; set; }

    [ValidateNever]
    public virtual Booking Booking { get; set; } = null!;

    [ValidateNever]
    public virtual Extraasset ExtraAsset { get; set; } = null!;
}
