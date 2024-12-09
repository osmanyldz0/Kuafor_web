using System;
using System.Collections.Generic;

namespace Kuafor_web.Models;

public partial class Salon
{
    public int SalonId { get; set; }

    public string? SalonAd { get; set; }

    public virtual ICollection<Berber> Berbers { get; set; } = new List<Berber>();
}
