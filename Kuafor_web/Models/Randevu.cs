using System;
using System.Collections.Generic;

namespace Kuafor_web.Models;

public partial class Randevu
{
    public int RandevuId { get; set; }

    public bool? Aktif { get; set; }

    public bool? Pasif { get; set; }

    public int? BerberId { get; set; }

    public int KullaniciId { get; set; }

    public TimeOnly RandevuSaat { get; set; }

    public string? Hizmetler { get; set; }

    public DateOnly RandevuTarih { get; set; }

    public TimeOnly RandevuBitis { get; set; }

    public TimeOnly HizmetSuresi { get; set; }

    public string? BerberAd { get; set; }

    public bool? RandevuOnay { get; set; }

    public int? Ucret { get; set; }

    public virtual Berber? Berber { get; set; }
}
