using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kuafor_web.Models;

public partial class Randevu
{
    [Display(Name = "Randevu Id")]
    public int RandevuId { get; set; }

    public bool? Aktif { get; set; }

    public bool? Pasif { get; set; }

    [Display(Name = "Randevu onay")]
    public bool? RandevuOnay { get; set; }
    [Display(Name = "Berber Id")]
    public int? BerberId { get; set; }
    [Display(Name = "Kullanıcı Id")]
    public int KullaniciId { get; set; }
    [Display(Name = "Randevu saati")]
    public TimeOnly RandevuSaat { get; set; }
    [Display(Name = "Hizmetler")]
    public string? Hizmetler { get; set; }
    [Display(Name = "Berber adı")]
    public string? BerberAd { get; set; }
    [Display(Name = "Randevu tarihi")]
    public DateOnly RandevuTarih { get; set; }
    [Display(Name = "Randevu Bitiş")]
    public TimeOnly RandevuBitis { get; set; }
    [Display(Name = "Hizmet Süresi")]
    public TimeOnly HizmetSuresi { get; set; }
    public int? Ucret { get; set; }
    public virtual Berber? Berber { get; set; }

  
}
