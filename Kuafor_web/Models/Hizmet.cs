using System;
using System.Collections.Generic;

namespace Kuafor_web.Models;

public partial class Hizmet
{
    public int HizmetId { get; set; }

    public string HizmetAdi { get; set; } = null!;

    public int HizmetUcreti { get; set; }

    public int KullaniciId { get; set; }

    public int BerberId { get; set; }

    public int HizmetSuresi { get; set; }

    public virtual Berber Berber { get; set; } = null!;

    public virtual Kullanici Kullanici { get; set; } = null!;
}
