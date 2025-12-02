using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatrioshkaBookingSystem.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int FloorId { get; set; }

    public int TypeId { get; set; }

    public string? RoomStatus { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    [ValidateNever]
    public virtual Floor? Floor { get; set; } = null!;
    [ValidateNever]
    public virtual Roomtype? Type { get; set; } = null!;
    [ValidateNever]
    public virtual ICollection<Roomasset> Assets { get; set; } = new List<Roomasset>();
}
