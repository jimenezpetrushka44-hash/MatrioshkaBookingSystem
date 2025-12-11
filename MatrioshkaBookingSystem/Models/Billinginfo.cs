using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatrioshkaBookingSystem.Models;

public partial class Billinginfo
{
    public int BillingId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string BillingAddress { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;


    [Required]
    [CreditCard]
    public string CardNumber { get; set; } = null!;

    [Required]
    [StringLength(5)] 
    public string ExpirationDate { get; set; } = null!;


    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User User { get; set; } = null!;
}