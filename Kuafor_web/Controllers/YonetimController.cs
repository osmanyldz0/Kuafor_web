using Kuafor_web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Kuafor_web.Models;

namespace berber4.Controllers
{
    [Authorize(Roles = "Yonetici")]
    public class YonetimController : Controller
    {

        BerberDbContext db = new BerberDbContext();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RandevuAl()
        {




            return View();
        }






        public IActionResult RandevuIptal()
        {
            return View();
        }







        public IActionResult Bilgilerim()
        {
            int kulId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var kullanici = db.Kullanicis.Find(kulId);
            kullanici.Parola = "";
            return View(kullanici);
        }
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





        public IActionResult CikisYap()
        {
            return View();
        }







        ///////










        public IActionResult Kullanicilar()
        {
            var kullanicilar = db.Kullanicis.Where(s => s.Silindi == false).OrderBy(s => s.Yetki).OrderBy(s => s.KullaniciIsim).ToList();
            return View(kullanicilar);
        }
        public IActionResult BerberGoster()
        {
            var berberler = db.Berbers.Where(s => s.Pasif == false).OrderBy(s => s.BerberIsim).ToList();
            return View(berberler);
        }
        public IActionResult SalonGoster()
        {
            var salonlar = db.Salons.ToList();
            return View(salonlar);
        }

        public IActionResult BerberGetir(int id)
        {
            var berber = db.Berbers.Where(s => s.Pasif == false && s.BerberId == id).FirstOrDefault();


            return View("BerberGuncelle", berber);
        }
        public IActionResult SalonGetir(int id)
        {
            var salon = db.Salons.Where(s => s.SalonId == id).FirstOrDefault();


            return View("SalonGuncelle", salon);
        }

        public IActionResult SalonGuncelle(Salon sal)

        {

            var salon = db.Salons.Where(s => s.SalonId == sal.SalonId).FirstOrDefault();






            salon.SalonId = sal.SalonId;
            salon.SalonAd = sal.SalonAd;




            db.Salons.Update(salon);
            db.SaveChanges();

            return RedirectToAction("SalonGoster");
        }
        public IActionResult BerberGuncelle(Berber ber)

        {


            var berber = db.Berbers.Where(s => s.Pasif == false && s.BerberId == ber.BerberId).FirstOrDefault();




            berber.BerberKazanc = ber.BerberKazanc;
            berber.Aktif = ber.Aktif;
            //   berber.SalonAd = ber.SalonAd;
            berber.SalonId = ber.SalonId;
            berber.IsBaslangicTarihi = ber.IsBaslangicTarihi;

            var salon_bul = db.Salons.Where(s => s.SalonId == berber.SalonId).FirstOrDefault();
            berber.CalisilmayanGun = ber.CalisilmayanGun;
            berber.SalonAd = salon_bul.SalonAd;
            berber.BerberId = ber.BerberId;
            berber.VerilenHizmetler = ber.VerilenHizmetler;
            berber.IsBitisSaati = ber.IsBitisSaati;
            berber.IsBaslangicSaati = ber.IsBaslangicSaati;




            db.Berbers.Update(berber);
            db.SaveChanges();

            return RedirectToAction("BerberGoster");
        }

        public IActionResult BerberSil(int id)

        {

            var berber = db.Berbers.Where(s => s.Pasif == false && s.BerberId == id).FirstOrDefault();




            berber.Pasif = true;
            // sayfa.Aktif = false;


            db.Berbers.Update(berber);
            db.SaveChanges();

            return RedirectToAction("BerberGoster");

        }
        public IActionResult SalonEkle() //httpget

        {



            return View();
        }
        [HttpPost]
        public IActionResult SalonEkle(Salon s) //httpget

        {


            db.Salons.Add(s);

            db.SaveChanges();


            return RedirectToAction("SalonGoster");
        }
        public IActionResult BerberEkle() //httpget

        {



            return View();
        }




        [HttpPost]
        public IActionResult BerberEkle(Berber b) //httpget

        {
            if (ModelState.IsValid)
            {
                // Veritabanından SalonId'ye göre SalonAd'ı bul
                var salon = db.Salons.FirstOrDefault(s => s.SalonId == b.SalonId);
                if (salon != null)
                {
                    b.SalonAd = salon.SalonAd; // SalonAd'ı Berber nesnesine ata
                }

            }
            b.BerberKazanc = 0;
            b.Pasif = false;

            db.Berbers.Add(b);

            db.SaveChanges();


            return RedirectToAction("BerberGoster");
        }
        public IActionResult KullaniciEkle() //httpget

        {



            return View();
        }
        [HttpPost]
        public IActionResult KullaniciEkle(Kullanici k) //httpget

        {
            k.Silindi = false;
            k.Parola = MD5Sifrele(k.Parola.Trim());
            db.Kullanicis.Add(k);

            db.SaveChanges();


            return RedirectToAction("Kullanicilar");
        }



        public IActionResult KullaniciGetir(int id)

        {

            var kullanici = db.Kullanicis.Where(s => s.Silindi == false && s.KullaniciId == id).FirstOrDefault();


            return View("KullaniciGuncelle", kullanici);
        }


        public IActionResult KullaniciGuncelle(Kullanici kul)

        {

            var kullanici = db.Kullanicis.Where(s => s.Silindi == false && s.KullaniciId == kul.KullaniciId).FirstOrDefault();






            kullanici.Aktif = kul.Aktif;

            kullanici.KullaniciIsim = kul.KullaniciIsim;
            kullanici.Eposta = kul.Eposta;
            kullanici.Telefon = kul.Telefon;

            kullanici.Yetki = kul.Yetki;
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

            return RedirectToAction("Kullanicilar");
        }




        public IActionResult KullaniciSil(int id)
        {
            // Kullanıcının randevuları var mı kontrol et
            var randevuKontrol = db.Randevus.Where(k => k.KullaniciId == id).ToList();

            // Eğer randevu varsa, silinemez
            if (randevuKontrol.Any())
            {
                ModelState.AddModelError("", "Kullanıcının randevusu bulunduğundan silinemez");


                return RedirectToAction("Kullanicilar");
            }

            // Eğer randevu yoksa, kullanıcıyı sil
            var kullanici = db.Kullanicis.Where(s => s.Silindi == false && s.KullaniciId == id).FirstOrDefault();
            if (kullanici != null)
            {
                kullanici.Silindi = true; // Silindi bayrağını güncelle
                db.Kullanicis.Update(kullanici);
                db.SaveChanges();
            }

            return RedirectToAction("Kullanicilar");
        }



        public IActionResult SalonSil(int id)

        {

            var salon = db.Salons.Where(s => s.SalonId == id).FirstOrDefault();







            db.Salons.Remove(salon);
            db.SaveChanges();

            return RedirectToAction("SalonGoster");
        }
        public IActionResult RandevuOnay()

        {






            var Randevu = db.Randevus.Where(r => r.Aktif == true).OrderBy(r => r.RandevuOnay).ToList();
            //  var kulllanici=   db.Kullanicis.Where(k => k.KullaniciId == kullaniciId).FirstOrDefault();

            return View(Randevu);


        }



        public IActionResult Onay(int id)
        {


            var bulunanRandevu = db.Randevus.FirstOrDefault(m => m.RandevuId == id);
            bulunanRandevu.RandevuOnay = true;
            db.Randevus.Update(bulunanRandevu);
            db.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("RandevuOnay"); // Kullanıcıyı listeye geri yönlendir


        }
        public IActionResult RandevuSil(int id)
        {


            var bulunanRandevu = db.Randevus.FirstOrDefault(m => m.RandevuId == id);

            db.Randevus.Remove(bulunanRandevu);
            db.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("RandevuOnay"); // Kullanıcıyı listeye geri yönlendir


        }



        public static string MD5Sifrele(string sifrelenecekMetin)
        {

            // MD5CryptoServiceProvider sınıfının bir örneğini oluşturduk.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //Parametre olarak gelen veriyi byte dizisine dönüştürdük.
            byte[] dizi = Encoding.UTF8.GetBytes(sifrelenecekMetin);
            //dizinin hash'ini hesaplattık.
            dizi = md5.ComputeHash(dizi);
            //Hashlenmiş verileri depolamak için StringBuilder nesnesi oluşturduk.
            StringBuilder sb = new StringBuilder();
            //Her byte'i dizi içerisinden alarak string türüne dönüştürdük.

            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            //hexadecimal(onaltılık) stringi geri döndürdük.
            return sb.ToString();
        }

    }




}

