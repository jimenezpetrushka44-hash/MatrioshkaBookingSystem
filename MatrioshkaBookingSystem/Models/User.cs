using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<Billinginfo> Billinginfos { get; set; } = new List<Billinginfo>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
