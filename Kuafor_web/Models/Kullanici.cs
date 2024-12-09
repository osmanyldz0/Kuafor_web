using System;
using System.Collections.Generic;

namespace Kuafor_web.Models;

public partial class Kullanici
{
    public int KullaniciId { get; set; }

    public string KullaniciIsim { get; set; } = null!;

    public string Eposta { get; set; } = null!;

    public string Telefon { get; set; } = null!;

    public string Parola { get; set; } = null!;

    public bool? Yetki { get; set; }

    public bool? Aktif { get; set; }

    public bool? Silindi { get; set; }

    public virtual ICollection<Hizmet> Hizmets { get; set; } = new List<Hizmet>();
}
