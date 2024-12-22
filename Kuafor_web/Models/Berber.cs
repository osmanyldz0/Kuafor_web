using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kuafor_web.Models;

public partial class Berber
{
    [Display(Name = "Id")]
    public int BerberId { get; set; }

    [Required(ErrorMessage = "Berber adı zorunludur.")]
    [Display(Name = "Berber İsim")]
    public string BerberIsim { get; set; } = null!;
    [Required(ErrorMessage = "zorunlu.")]
    [Display(Name = "Aktiflik")]
    public bool? Aktif { get; set; }

    public bool? Pasif { get; set; }
    [Required(ErrorMessage = "Hizmet alanı zorunludur.")]
    [Display(Name = "Hizmetler")]
    public string VerilenHizmetler { get; set; } = null!;
    [Display(Name = "İşbaşı")]
    [Required(ErrorMessage = "zorunlu.")]
    public TimeOnly? IsBaslangicSaati { get; set; }
    [Display(Name = "Vardiya sonu")]
    [Required(ErrorMessage = "zorunlu.")]
    public TimeOnly? IsBitisSaati { get; set; }

    [Display(Name = "Salon")]
    [Required(ErrorMessage = "zorunluuu.")]
    public int? SalonId  { get; set; }

    [Display(Name = "Salon")]
     
    public string? SalonAd { get; set; }
    [Display(Name = "İzin Günü")]
    [Required(ErrorMessage = "zorunlu.")]
    public string CalisilmayanGun { get; set; }
    [Display(Name = "Toplam kazanç")]
    
    public int? Berberkazanc { get; set; }
    [Display(Name = "İşe Başlama Tarihi")]
    [Required(ErrorMessage = "zorunlu.")]
    public DateOnly? IsBaslangicTarihi { get; set; }
    public virtual ICollection<Hizmet> Hizmets { get; set; } = new List<Hizmet>();

    public virtual ICollection<Randevu> Randevus { get; set; } = new List<Randevu>();

    public virtual Salon? Salon { get; set; }
}
