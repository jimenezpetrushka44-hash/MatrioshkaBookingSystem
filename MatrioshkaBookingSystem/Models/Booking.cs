using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? RoomId { get; set; }

    public int? BillingId { get; set; }

    public DateOnly DateofBooking { get; set; }

    public DateOnly EndofBooking { get; set; }

    public virtual Billinginfo? Billing { get; set; }

    public virtual ICollection<Bookingextraasset> Bookingextraassets { get; set; } = new List<Bookingextraasset>();

    public virtual Room? Room { get; set; }

    public virtual User? User { get; set; }
}
