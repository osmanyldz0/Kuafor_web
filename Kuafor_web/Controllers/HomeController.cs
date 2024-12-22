using Kuafor_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace berber4.Controllers
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
            var sayfa = db.Sayfalars.Where(a => a.Silindi == false && a.Aktif == true && a.SayfaId == id).FirstOrDefault(); //tek kayýt geldiðinden emin olmak için firsor default

            return View(sayfa);
        }
        //public IActionResult RandevuCakisiyorMu(Randevu yeniRandevu)
        //{

        //    var mevcutRandevular=db.Randevus.Where(r =>r.RandevuTarih==yeniRandevu.RandevuTarih && ).Fir
        //}

     //   private readonly string _apiKey = "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";
        private static readonly string ApiKey = "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"; // API anahtarýnýzý buraya ekleyin
        private static readonly string ApiEndpoint = "https://api.groq.com/openai/v1/chat/completions"; // API endpoint adresi








        public async Task<IActionResult> Tavsiye(IFormFile photo)
        {



            if (photo != null && photo.Length > 0)
            {
                // string apiKey = _apiKey; // API anahtarýnýz burada
              
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);



                // Fotoðrafý Base64 formatýna çevirin
                string base64Image;
                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    byte[] byteArray = memoryStream.ToArray();
                    base64Image = Convert.ToBase64String(byteArray);
                }

                // JSON Mesajý Oluþturma
                var messagePayload = new
                {
                    model = "llama-3.2-11b-vision-preview",
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = "Can you give me some suggestions about hair style and hair color for the person in this photo? " },
                                new
                                {
                                    type = "image_url",
                                    image_url = new { url = $"data:image/jpeg;base64,{base64Image}" }
                                }
                            }
                        }
                    }
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                    var jsonPayload = JsonConvert.SerializeObject(messagePayload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(ApiEndpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JObject.Parse(responseContent);
                        var message = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();

                        // Yanýtý ViewBag'e ata
                        //ViewBag.ResponseMessage = message;
                        ViewBag.EditedImageUrl = message;
                        return View();
                    }
                    ViewBag.EditedImageUrl = "API isteði baþarýsýz";

                    return View();
                }

                // API'ye istek gönderme
            }

            else
            {
                ViewBag.Error = " fotoðraf yükleyiniz .";
            }

            return View();
        }


    



















        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Kaydol() //httpget

        {



            return View();
        }
        [HttpPost]
        public IActionResult Kaydol(Kullanici k) //httpget

        {
            k.Silindi = false;
            k.Aktif = true;
            k.Yetki = false;
            k.Parola = MD5Sifrele(k.Parola.Trim());
            db.Kullanicis.Add(k);

            db.SaveChanges();


            return Redirect("/Giris/GirisYap");
        }


        public IActionResult Hakkimizda()
        {


            return View();
        }

        public IActionResult Subeler()
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

        // Randevu alýrken saat kontrolü yapacak action

        [Authorize]
        public IActionResult RandevuAl(Randevu r)
        {
            // Gerekli alanlarýn boþ olup olmadýðýný kontrol edin
            if (r.RandevuTarih == null || r.RandevuSaat == null || r.BerberId == null || string.IsNullOrEmpty(r.Hizmetler))
            {
                ModelState.AddModelError("", "Lütfen tüm alanlarý doldurduðunuzdan emin olun.");
                return View("Randevu");
            }
            //Randevu r = new Randevu();
            r.Aktif = true;
            r.Pasif = false;
            var berber = db.Berbers.Where(x => x.BerberId == r.BerberId).FirstOrDefault();
            // Hizmet süresini belirle
            if (r.Hizmetler == "Saç")
            {
                r.HizmetSuresi = new TimeOnly(0, 40); // 00:40
                r.Ucret = 300;
                berber.Berberkazanc += 300;
            }
            else if (r.Hizmetler == "Sakal")
            {
                r.HizmetSuresi = new TimeOnly(0, 20); // 00:20
                r.Ucret = 150;
                berber.Berberkazanc += 150;
            }
            else if (r.Hizmetler == "Bakým")
            {
                r.HizmetSuresi = new TimeOnly(0, 20); // 00:20
                r.Ucret = 200;
                berber.Berberkazanc += 200;
            }
            else if (r.Hizmetler == "Saç ve Sakal")
            {
                r.HizmetSuresi = new TimeOnly(1, 0); // 01:00
                r.Ucret = 450;
                berber.Berberkazanc += 450;
            }
            else if (r.Hizmetler == "Saç ve Bakým")
            {
                r.HizmetSuresi = new TimeOnly(1, 5); // 01:05
                r.Ucret = 500;
                berber.Berberkazanc += 500;

            }
            else if (r.Hizmetler == "Sakal ve Bakým")
            {
                r.HizmetSuresi = new TimeOnly(0, 45); // 00:45
                r.Ucret = 350;
                berber.Berberkazanc += 350;
            }
            else if (r.Hizmetler == "Saç, Sakal ve Bakým")
            {
                r.HizmetSuresi = new TimeOnly(1, 25); // 01:25
                r.Ucret = 600;
                berber.Berberkazanc += 600;
            }

            // Hizmet süresini TimeSpan'e çevir
            TimeSpan span = r.HizmetSuresi.ToTimeSpan();
            TimeOnly toplam = r.RandevuSaat.Add(span);
            r.RandevuBitis = toplam;
            // Seçilen tarihteki randevularý al
            //var mevcutRandevular = db.Randevus
            //    .Where(x => x.RandevuTarih == r.RandevuTarih)
            //    .ToList();

            var mevcutRandevular = db.Randevus
    .Where(x => x.RandevuTarih == r.RandevuTarih && x.BerberId == r.BerberId)
    .ToList();

            //                                                                                                15.00 15.40
            // Seçilen saat diliminde baþka bir randevu olup olmadýðýný kontrol et                           16.30  17.00
            foreach (var mevcutRandevu in mevcutRandevular)
            {
                if ((r.RandevuSaat >= mevcutRandevu.RandevuSaat && r.RandevuSaat < mevcutRandevu.RandevuBitis) ||
          (r.RandevuBitis > mevcutRandevu.RandevuSaat && r.RandevuBitis <= mevcutRandevu.RandevuBitis))
                {
                    string doluSaatler = $"{mevcutRandevu.RandevuSaat:HH:mm} - {mevcutRandevu.RandevuBitis:HH:mm}";
                    ModelState.AddModelError("", $"Seçtiðiniz berber, {doluSaatler} saatleri arasýnda baþka bir randevusu olduðu için bu saat diliminde randevu alýnamaz.");
                    return View("Randevu");
                }
            }

            

            // Berberin çalýþma saatleri kontrolü
            if (r.RandevuSaat < berber.IsBaslangicSaati || r.RandevuSaat >= berber.IsBitisSaati)
            {
                string calismaSaatleri = $"{berber.IsBaslangicSaati:HH:mm} - {berber.IsBitisSaati:HH:mm}";

               
                ModelState.AddModelError("", $"Seçtiðiniz berberin çalýþma saatleri {calismaSaatleri} arasýndadýr. Lütfen belirtilen saatler arasýnda bir zaman dilimi seçiniz." );
                return View("Randevu");
            }

            if (r.RandevuTarih.DayOfWeek.ToString().ToUpper() == berber.CalisilmayanGun.ToUpper())
            {
                string randevuGunu = r.RandevuTarih.ToString("dddd", new CultureInfo("tr-TR")); // Türkçe gün adý
                ModelState.AddModelError("", $"Seçtiðiniz berber {randevuGunu} günü çalýþmýyor. Lütfen baþka bir gün seçiniz.");
                return View("Randevu"); // Kullanýcýyý randevu formuna geri yönlendir
            }


            if (r.Hizmetler == "Saç")
            {
                //  var berber = db.Berbers.Where(x => x.BerberId == r.BerberId).FirstOrDefault();



                if (berber.VerilenHizmetler == "Sakal,Bakým/Sakal/Bakým" || berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Bakým")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Seçtiðiniz berber, saç kesim hizmeti vermemektedir. Bu berberden alabileceðiniz hizmetler þunlardýr : {islemler}");
                    return View("Randevu");
                }

            }
            else if (r.Hizmetler == "Sakal")
            {




                if (berber.VerilenHizmetler == "Saç,Bakým/Saç/Bakým" || berber.VerilenHizmetler == "Saç" || berber.VerilenHizmetler == "Bakým")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Seçtiðiniz berber, sakal kesim hizmeti vermemektedir. Bu berberden alabileceðiniz hizmetler þunlardýr : {islemler}");
                    return View("Randevu");
                }


            }


            else if (r.Hizmetler == "Bakým")
            {




                if (berber.VerilenHizmetler == "Saç,Sakal /Saç/Sakal" || berber.VerilenHizmetler == "Saç" || berber.VerilenHizmetler == "Sakal")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Seçtiðiniz berber, bakým hizmeti vermemektedir. Bu berberden alabileceðiniz hizmetler þunlardýr : {islemler}");
                    return View("Randevu");
                }


            }




            else if (r.Hizmetler == "Saç ve Sakal")
            {




                if (berber.VerilenHizmetler == "Bakým" || berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Saç" || berber.VerilenHizmetler == "Saç,Bakým/Saç/Bakým" || berber.VerilenHizmetler == "Sakal,Bakým/Sakal/Bakým")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Seçtiðiniz berber, sadece  {islemler}   hizmeti vermektedir.");
                    return View("Randevu");
                }


            }
            else if (r.Hizmetler == "Saç ve Bakým")
            {




                if (berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Bakým" || berber.VerilenHizmetler == "Saç" || berber.VerilenHizmetler == "Saç,Sakal /Saç/Sakal" || berber.VerilenHizmetler == "Sakal,Bakým/Sakal/Bakým")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Seçtiðiniz berber, sadece  {islemler}   hizmeti vermektedir.");
                    return View("Randevu");
                }


            }



            else if (r.Hizmetler == "Sakal ve Bakým")
            {



                if (berber.VerilenHizmetler == "Sakal" || berber.VerilenHizmetler == "Bakým" || berber.VerilenHizmetler == "Saç" || berber.VerilenHizmetler == "Saç,Bakým/Saç/Bakým" || berber.VerilenHizmetler == "Saç,Sakal/Saç/Sakal")
                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Seçtiðiniz berber, sadece  {islemler}   hizmeti vermektedir.");
                    return View("Randevu");
                }


            }

            else if (r.Hizmetler == "Saç, Sakal ve Bakým")
            {




                if (berber.VerilenHizmetler == "Saç" || berber.VerilenHizmetler == "Sakal" ||
                    berber.VerilenHizmetler == "Bakým" || berber.VerilenHizmetler == "Saç,Bakým/Saç/Bakým"
                    || berber.VerilenHizmetler == "Saç,Sakal /Saç/Sakal" || berber.VerilenHizmetler == "Sakal,Bakým/Sakal/Bakým")

                {
                    string islemler = $"{berber.VerilenHizmetler}";
                    ModelState.AddModelError("", $"Seçtiðiniz berberden alabileceðiniz hizmetler þunlardýr : {islemler}");
                    return View("Randevu");
                }


            }

            var kullaniciId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            r.KullaniciId = kullaniciId;
            // var berber2 = db.Berbers.Where(b => b.BerberId == r.BerberId).FirstOrDefault();
            r.Berber = berber;
            r.BerberAd = berber.BerberIsim;
            r.RandevuOnay = false;

            // Randevuyu veritabanýna ekle
            db.Randevus.Add(r);
            db.SaveChanges();

            return RedirectToAction("RandevuGoster", "Home");
        }




        public IActionResult Berberler()
        {
            var berberler = db.Berbers.Where(s => s.Pasif == false).ToList();

            // Debug: Veritabanýndan çekilen veriyi loglayýn
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
            var Randevu = db.Randevus.Where(r => r.KullaniciId == kullaniciId&&r.RandevuOnay==true).ToList();
          //  var kulllanici=   db.Kullanicis.Where(k => k.KullaniciId == kullaniciId).FirstOrDefault();

            return View(Randevu);
        }

        public IActionResult RandevuGuncelle(Randevu r)

        {


            
            var randevu = db.Randevus.Where(s => s.RandevuId == r.RandevuId).FirstOrDefault();
            var berber = db.Berbers.Where(s => s.BerberId == r.BerberId).FirstOrDefault();

            berber.Berberkazanc -= r.Ucret;

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
            var randevu = db.Randevus.Where(s =>   s.RandevuId == id).FirstOrDefault();


            return RedirectToAction("RandevuGuncelle", randevu);
        }


        public IActionResult RandevuSil(int id)
        {


            var bulunanRandevu = db.Randevus.FirstOrDefault(m => m.RandevuId == id);
            var berber = db.Berbers.Where(s => s.BerberId == bulunanRandevu.BerberId).FirstOrDefault();

            berber.Berberkazanc -= bulunanRandevu.Ucret;
            db.Randevus.Remove(bulunanRandevu);
            db.SaveChanges(); // Deðiþiklikleri kaydet

            return RedirectToAction("RandevuGoster"); // Kullanýcýyý listeye geri yönlendir


        }



        public static string MD5Sifrele(string sifrelenecekMetin)
        {

            // MD5CryptoServiceProvider sýnýfýnýn bir örneðini oluþturduk.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //Parametre olarak gelen veriyi byte dizisine dönüþtürdük.
            byte[] dizi = Encoding.UTF8.GetBytes(sifrelenecekMetin);
            //dizinin hash'ini hesaplattýk.
            dizi = md5.ComputeHash(dizi);
            //Hashlenmiþ verileri depolamak için StringBuilder nesnesi oluþturduk.
            StringBuilder sb = new StringBuilder();
            //Her byte'i dizi içerisinden alarak string türüne dönüþtürdük.

            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            //hexadecimal(onaltýlýk) stringi geri döndürdük.
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
