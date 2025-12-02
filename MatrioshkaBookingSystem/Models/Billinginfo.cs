using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Billinginfo
{
    public int BillingId { get; set; }

    public int UserId { get; set; }

    public string CardNumber { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User User { get; set; } = null!;
}
