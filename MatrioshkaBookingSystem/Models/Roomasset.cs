using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace MatrioshkaBookingSystem.Models;

public partial class Roomasset
{
    public int AssetId { get; set; }

    public string? AssetName { get; set; }

    public string AssetStatus { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
