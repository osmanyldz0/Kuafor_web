using System;
using System.Collections.Generic;

namespace Kuafor_web.Models;

public partial class Berber
{
    public int BerberId { get; set; }

    public string BerberIsim { get; set; } = null!;

    public bool? Aktif { get; set; }

    public bool? Pasif { get; set; }

    public string VerilenHizmetler { get; set; } = null!;

    public TimeOnly? IsBaslangicSaati { get; set; }

    public TimeOnly? IsBitisSaati { get; set; }

    public int? SalonId { get; set; }

    public string? SalonAd { get; set; }

    public string CalisilmayanGun { get; set; } = null!;

    public int? BerberKazanc { get; set; }

    public DateOnly? IsBaslangicTarihi { get; set; }

    public virtual ICollection<Hizmet> Hizmets { get; set; } = new List<Hizmet>();

    public virtual ICollection<Randevu> Randevus { get; set; } = new List<Randevu>();

    public virtual Salon? Salon { get; set; }
}
