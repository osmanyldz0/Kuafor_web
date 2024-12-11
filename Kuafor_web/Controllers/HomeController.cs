using Kuafor_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Kuafor_web.Models;

namespace Kuafor_web.Controllers
{
    public class HomeController : Controller
    {
        BerberDbContext db = new BerberDbContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int id)
        {

            BerberDbContext db = new BerberDbContext();
            var sayfa = db.Sayfalars.Where(a => a.Silindi == false && a.Aktif == true && a.SayfaId == id).FirstOrDefault(); //tek kay�t geldi�inden emin olmak i�in firsor default

            return View(sayfa);
        }
        //public IActionResult RandevuCakisiyorMu(Randevu yeniRandevu)
        //{

        //    var mevcutRandevular=db.Randevus.Where(r =>r.RandevuTarih==yeniRandevu.RandevuTarih && ).Fir
        //}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public IActionResult Hakkimizda()
        {


            return View();
        }



        public IActionResult Iletisim()
        {


            return View();
        }
        public IActionResult Anasayfa()
        {


            return View();
        }


        public IActionResult Randevu()
        {


            return View();

        }


        public IActionResult UserSayfasi()
        {


            return View();

        }


        [Authorize]
        public IActionResult Bilgilerim()
        {
            int kulId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var kullanici = db.Kullanicis.Find(kulId);
            kullanici.Parola = "";
            return View(kullanici);
        }
        [Authorize]
        public IActionResult BilgilerimiGuncelle(Kullanici kul)
        {
            var kullanici = db.Kullanicis.Where(s => s.Silindi == false && s.KullaniciId == kul.KullaniciId).FirstOrDefault();



            kullanici.KullaniciIsim = kul.KullaniciIsim;
            kullanici.Eposta = kul.Eposta;
            kullanici.Telefon = kul.Telefon;

            try
            {
                if (kul.Parola.Trim().Length != 0)
                {

                    kullanici.Parola = MD5Sifrele(kul.Parola.Trim());



                }


            }

            catch { }

            db.Kullanicis.Update(kullanici);
            db.SaveChanges();

            return RedirectToAction("Bilgilerim");
        }

        // Randevu al�rken saat kontrol� yapacak action

        [Authorize]
        public IActionResult RandevuAl(Randevu r)
        {
            // Gerekli alanlar�n bo� olup olmad���n� kontrol edin
            if (r.RandevuTarih == null || r.RandevuSaat == null || r.BerberId == null || string.IsNullOrEmpty(r.Hizmetler))
            {
                ModelState.AddModelError("", "L�tfen t�m alanlar� doldurdu�unuzdan emin olun.");
                return View("Randevu");
            }
            //Randevu r = new Randevu();
            r.Aktif = true;
            r.Pasif = false;
            var berber = db.Berbers.Where(x => x.BerberId == r.BerberId).FirstOrDefault();
            // Hizmet s�resini belirle
            if (r.Hizmetler == "Sa�")
            {
                r.HizmetSuresi = new TimeOnly(0, 40); // 00:40
                r.Ucret = 300;
                berber.BerberKazanc += 300;
            }
            else if (r.Hizmetler == "Sakal")
            {
                r.HizmetSuresi = new TimeOnly(0, 20); // 00:20
                r.Ucret = 150;
                berber.BerberKazanc += 150;
            }
            else if (r.Hizmetler == "Bak�m")
            {
                r.HizmetSuresi = new TimeOnly(0, 20); // 00:20
                r.Ucret = 200;
                berber.BerberKazanc += 200;
            }
            else if (r.Hizmetler == "Sa� ve Sakal")
            {
                r.HizmetSuresi = new TimeOnly(1, 0); // 01:00
                r.Ucret = 450;
                berber.BerberKazanc += 450;
            }
            else if (r.Hizmetler == "Sa� ve Bak�m")
            {
                r.HizmetSuresi = new TimeOnly(1, 5); // 01:05
                r.Ucret = 500;
                berber.BerberKazanc += 500;

            }
            else if (r.Hizmetler == "Sakal ve Bak�m")
            {
                r.HizmetSuresi = new TimeOnly(0, 45); // 00:45
                r.Ucret = 350;
                berber.BerberKazanc += 350;
            }
            else if (r.Hizmetler == "Sa�, Sakal ve Bak�m")
            {
                r.HizmetSuresi = new TimeOnly(1, 25); // 01:25
                r.Ucret = 600;
                berber.BerberKazanc += 600;
            }

            // Hizmet s�resini TimeSpan'e �evir
            TimeSpan span = r.HizmetSuresi.ToTimeSpan();
            TimeOnly toplam = r.RandevuSaat.Add(span);
            r.RandevuBitis = toplam;
            // Se�ilen tarihteki randevular� al
            //var mevcutRandevular = db.Randevus
            //    .Where(x => x.RandevuTarih == r.RandevuTarih)
            //    .ToList();

            var mevcutRandevular = db.Randevus
    .Where(x => x.RandevuTarih == r.RandevuTarih && x.BerberId == r.BerberId)
    .ToList();

            //                                                                                                15.00 15.40
            // Se�ilen saat diliminde ba�ka bir randevu olup olmad���n� kontrol et                           16.30  17.00
            foreach (var mevcutRandevu in mevcutRandevular)
            {
                if ((r.RandevuSaat >= mevcutRandevu.RandevuSaat && r.RandevuSaat < mevcutRandevu.RandevuBitis) ||
          (r.RandevuBitis > mevcutRandevu.RandevuSaat && r.RandevuBitis <= mevcutRandevu.RandevuBitis))
                {
                    string doluSaatler = $"{mevcutRandevu.RandevuSaat:HH:mm} - {mevcutRandevu.RandevuBitis:HH:mm}";
                    ModelState.AddModelError("", $"Se�ti�iniz berber, {doluSaatler} saatleri aras�nda ba�ka bir randevusu oldu�u i�in bu saat diliminde randevu al�namaz.");
                    return View("Randevu");
                }
            }



            // Berberin �al��ma saatleri kontrol�
            if (r.RandevuSaat < berber.IsBaslangicSaati || r.RandevuSaat >= berber.IsBitisSaati)
            {
                string calismaSaatleri = $"{berber.IsBaslangicSaati:HH:mm} - {berber.IsBitisSaati:HH:mm}";


                ModelState.AddModelError("", $"Se�ti�iniz berberin �al��ma saatleri {calismaSaatleri} aras�ndad�r. L�tfen belirtilen saatler aras�nda bir zaman dilimi se�iniz.");
                return View("Randevu");
            }

            if (r.RandevuTarih.DayOfWeek.ToString().ToUpper() == berber.CalisilmayanGun.ToUpper())
            {
                string randevuGunu = r.RandevuTarih.ToString("dddd", new CultureInfo("tr-TR")); // T�rk�e g�n ad�
                ModelState.AddModelError("", $"Se�ti�iniz berber {randevuGunu} g�n� �al��m�yor. L�tfen ba�ka bir g�n se�iniz.");
                return View("Randevu"); // Kullan�c�y� randevu formuna geri y�nlendir
            }


            if (r.Hizmetler == "Sa�")
            {
                //  var berber = db.Berbers.Where(x => x.BerberId == r.BerberId).FirstOrDefault();



                if (berber.VerilenHizmetler == "Sakal,Bak�m/Sakal/Bak�m" || berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Bak�m")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Se�ti�iniz berber, sa� kesim hizmeti vermemektedir. Bu berberden alabilece�iniz hizmetler �unlard�r : {islemler}");
                    return View("Randevu");
                }

            }
            else if (r.Hizmetler == "Sakal")
            {




                if (berber.VerilenHizmetler == "Sa�,Bak�m/Sa�/Bak�m" || berber.VerilenHizmetler == "Sa�" || berber.VerilenHizmetler == "Bak�m")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Se�ti�iniz berber, sakal kesim hizmeti vermemektedir. Bu berberden alabilece�iniz hizmetler �unlard�r : {islemler}");
                    return View("Randevu");
                }


            }


            else if (r.Hizmetler == "Bak�m")
            {




                if (berber.VerilenHizmetler == "Sa�,Sakal /Sa�/Sakal" || berber.VerilenHizmetler == "Sa�" || berber.VerilenHizmetler == "Sakal")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Se�ti�iniz berber, bak�m hizmeti vermemektedir. Bu berberden alabilece�iniz hizmetler �unlard�r : {islemler}");
                    return View("Randevu");
                }


            }




            else if (r.Hizmetler == "Sa� ve Sakal")
            {




                if (berber.VerilenHizmetler == "Bak�m" || berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Sa�" || berber.VerilenHizmetler == "Sa�,Bak�m/Sa�/Bak�m" || berber.VerilenHizmetler == "Sakal,Bak�m/Sakal/Bak�m")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Se�ti�iniz berber, sadece  {islemler}   hizmeti vermektedir.");
                    return View("Randevu");
                }


            }
            else if (r.Hizmetler == "Sa� ve Bak�m")
            {




                if (berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Bak�m" || berber.VerilenHizmetler == "Sa�" || berber.VerilenHizmetler == "Sa�,Sakal /Sa�/Sakal" || berber.VerilenHizmetler == "Sakal,Bak�m/Sakal/Bak�m")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Se�ti�iniz berber, sadece  {islemler}   hizmeti vermektedir.");
                    return View("Randevu");
                }


            }



            else if (r.Hizmetler == "Sakal ve Bak�m")
            {



                if (berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Bak�m" || berber.VerilenHizmetler == "Sa�" || berber.VerilenHizmetler == "Sa�,Bak�m/Sa�/Bak�m" || berber.VerilenHizmetler == "Sa�,Sakal/Sa�/Sakal")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Se�ti�iniz berber, sadece  {islemler}   hizmeti vermektedir.");
                    return View("Randevu");
                }


            }

            else if (r.Hizmetler == "Sa�, Sakal ve Bak�m")
            {




                if (berber.VerilenHizmetler == "Sa�" || berber.VerilenHizmetler == "Sakal" ||
                    berber.VerilenHizmetler == "Bak�m" || berber.VerilenHizmetler == "Sa�,Bak�m/Sa�/Bak�m"
                    || berber.VerilenHizmetler == "Sa�,Sakal /Sa�/Sakal" || berber.VerilenHizmetler == "Sakal,Bak�m/Sakal/Bak�m")

                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Se�ti�iniz berberden alabilece�iniz hizmetler �unlard�r : {islemler}");
                    return View("Randevu");
                }


            }

            var kullaniciId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            r.KullaniciId = kullaniciId;
            // var berber2 = db.Berbers.Where(b => b.BerberId == r.BerberId).FirstOrDefault();
            r.Berber = berber;
            r.BerberAd = berber.BerberIsim;
            r.RandevuOnay = false;

            // Randevuyu veritaban�na ekle
            db.Randevus.Add(r);
            db.SaveChanges();

            return RedirectToAction("RandevuGoster", "Home");
        }




        public IActionResult Berberler()
        {
            var berberler = db.Berbers.Where(s => s.Pasif == false).ToList();

            // Debug: Veritaban�ndan �ekilen veriyi loglay�n
            foreach (var berber in berberler)
            {
                Console.WriteLine($"Berber Isim: {berber.BerberIsim}, Pasif: {berber.Pasif}");
            }

            return View(berberler);
        }




        public IActionResult HizmetSec(Randevu r)
        {
            var berberGorev = db.Berbers.Where(b => b.BerberId == r.BerberId).FirstOrDefault();

            ViewBag.berberGorev = berberGorev;
            return View();
        }

        public IActionResult RandevuGoster(int id)
        {
            var kullaniciId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var Randevu = db.Randevus.Where(r => r.KullaniciId == kullaniciId && r.RandevuOnay == true).ToList();
            //  var kulllanici=   db.Kullanicis.Where(k => k.KullaniciId == kullaniciId).FirstOrDefault();

            return View(Randevu);
        }

        public IActionResult RandevuGuncelle(Randevu r)

        {



            var randevu = db.Randevus.Where(s => s.RandevuId == r.RandevuId).FirstOrDefault();
            var berber = db.Berbers.Where(s => s.BerberId == r.BerberId).FirstOrDefault();

            berber.BerberKazanc -= r.Ucret;

            randevu.RandevuId = r.RandevuId;
            randevu.Aktif = r.Aktif;
            randevu.Pasif = r.Pasif;
            randevu.BerberId = r.BerberId;
            randevu.KullaniciId = r.KullaniciId;
            randevu.RandevuSaat = r.RandevuSaat;


            randevu.Hizmetler = r.Hizmetler;
            randevu.RandevuTarih = r.RandevuTarih;

            randevu.RandevuBitis = r.RandevuBitis;
            randevu.HizmetSuresi = r.HizmetSuresi;
            randevu.BerberAd = r.BerberAd;
            randevu.RandevuOnay = r.RandevuOnay;


            db.Randevus.Remove(randevu);
            db.SaveChanges();
            //Randevu YeniRandevu = new Randevu();

            return Redirect("/Home/Randevu");
        }
        public IActionResult RandevuGetir(int id)
        {
            var randevu = db.Randevus.Where(s => s.RandevuId == id).FirstOrDefault();


            return RedirectToAction("RandevuGuncelle", randevu);
        }


        public IActionResult RandevuSil(int id)
        {


            var bulunanRandevu = db.Randevus.FirstOrDefault(m => m.RandevuId == id);
            var berber = db.Berbers.Where(s => s.BerberId == bulunanRandevu.BerberId).FirstOrDefault();

            berber.BerberKazanc -= bulunanRandevu.Ucret;
            db.Randevus.Remove(bulunanRandevu);
            db.SaveChanges(); // De�i�iklikleri kaydet

            return RedirectToAction("RandevuGoster"); // Kullan�c�y� listeye geri y�nlendir


        }



        public static string MD5Sifrele(string sifrelenecekMetin)
        {

            // MD5CryptoServiceProvider s�n�f�n�n bir �rne�ini olu�turduk.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //Parametre olarak gelen veriyi byte dizisine d�n��t�rd�k.
            byte[] dizi = Encoding.UTF8.GetBytes(sifrelenecekMetin);
            //dizinin hash'ini hesaplatt�k.
            dizi = md5.ComputeHash(dizi);
            //Hashlenmi� verileri depolamak i�in StringBuilder nesnesi olu�turduk.
            StringBuilder sb = new StringBuilder();
            //Her byte'i dizi i�erisinden alarak string t�r�ne d�n��t�rd�k.

            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            //hexadecimal(onalt�l�k) stringi geri d�nd�rd�k.
            return sb.ToString();
        }






        public IActionResult KullaniciEkle() //httpget

        {



            return View();
        }
        [HttpPost]
        public IActionResult KullaniciEkle(Kullanici k) //httpget

        {
            k.Yetki = false;
            k.Aktif = true;
            k.Silindi = false;
            k.Parola = MD5Sifrele(k.Parola.Trim());
            db.Kullanicis.Add(k);

            db.SaveChanges();


            return Redirect("/Giris/GirisYap");
        }

        public IActionResult KullanicIslemleri(Kullanici k) //httpget

        {
            var kullaniciId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var kullanici = db.Kullanicis.Where(b => b.KullaniciId == kullaniciId).FirstOrDefault();
            bool yetki = (bool)kullanici.Yetki;

            if (yetki)
            {
                return Redirect("/Yonetim/Index");
            }
            else
            {
                return Redirect("/User/Index");

            }


        }

    }













}
