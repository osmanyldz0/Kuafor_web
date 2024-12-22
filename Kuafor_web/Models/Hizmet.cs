using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kuafor_web.Models;

public partial class Hizmet
{
    [Display(Name = "Hizmet Id")]
    public int HizmetId { get; set; }

    [Display(Name = "Hizmet adı")]
    public string HizmetAdi { get; set; } = null!;
    [Display(Name = "Hizmet ücreti")]
    public int HizmetUcreti { get; set; }
    [Display(Name = "Kullanıcı ıd")]
    public int KullaniciId { get; set; }
    [Display(Name = "Berber Id")]
    public int BerberId { get; set; }
    [Display(Name = "Hizmet süresi")]
    public int HizmetSuresi { get; set; }

    public virtual Berber Berber { get; set; } = null!;

    public virtual Kullanici Kullanici { get; set; } = null!;

    
}
