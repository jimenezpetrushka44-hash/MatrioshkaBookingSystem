using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Hotel
{
    public int HotelId { get; set; }

    public string HotelName { get; set; } = null!;

    public string HotelLocation { get; set; } = null!;

    public string HotelStatus { get; set; } = null!;
    public string? ImagePath { get; set; }


    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
