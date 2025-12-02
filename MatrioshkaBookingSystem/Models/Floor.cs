
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace MatrioshkaBookingSystem.Models;

public partial class Floor
{
    public int FloorId { get; set; }

    [Required]
    public int HotelId { get; set; }

    [Required] 
    public string FloorStatus { get; set; } = null!;

    [Required]
    public int FloorNumber { get; set; }


  
    [ValidateNever]
    public virtual Hotel? Hotel { get; set; } 

    [ValidateNever]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>(); 
}