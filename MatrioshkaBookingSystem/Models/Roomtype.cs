using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Roomtype
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal TypePrice { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
