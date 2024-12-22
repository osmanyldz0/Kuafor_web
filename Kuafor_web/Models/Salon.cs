using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kuafor_web.Models;

public partial class Salon
{
    [Display(Name = "Id")]
     
    [Required(ErrorMessage = "zorunlu.")]
    public int SalonId { get; set; }
    [Display(Name = "Salon Ad")]
    
    [Required(ErrorMessage = "zorunlu.")]
    public string? SalonAd { get; set; }

    public virtual ICollection<Berber> Berbers { get; set; } = new List<Berber>();
}
