using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Kuafor_web.Models;

public partial class Kullanici
{
    public int KullaniciId { get; set; }
 
    [Display(Name = "Ad-soyad")]
     

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string KullaniciIsim { get; set; } = null!;


     
    [Display(Name = "E-posta")]
    [Required(ErrorMessage = "E-posta zorunludur.")]
    public string Eposta { get; set; } = null!;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    public string Telefon { get; set; } = null!;

    [Required(ErrorMessage = "Parola zorunludur.")]
    public string Parola { get; set; } = null!;

    public bool? Yetki { get; set; }

    public bool? Aktif { get; set; }

    public bool? Silindi { get; set; }

    public virtual ICollection<Hizmet> Hizmets { get; set; } = new List<Hizmet>();
}
