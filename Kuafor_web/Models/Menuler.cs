using System;
using System.Collections.Generic;

namespace Kuafor_web.Models;

public partial class Menuler
{
    public int MenuId { get; set; }

    public string Baslık { get; set; } = null!;

    public int Sıra { get; set; }

    public int? UstId { get; set; }

    public bool? Aktif { get; set; }

    public bool? Silindi { get; set; }

    public string? Url { get; set; }

    public string? Icerik { get; set; }
}
