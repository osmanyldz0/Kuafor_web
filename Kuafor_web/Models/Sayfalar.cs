using System;
using System.Collections.Generic;

namespace Kuafor_web.Models;

public partial class Sayfalar
{
    public int SayfaId { get; set; }

    public string Baslık { get; set; } = null!;

    public string? İcerik { get; set; }

    public bool? Aktif { get; set; }

    public bool? Silindi { get; set; }
}
