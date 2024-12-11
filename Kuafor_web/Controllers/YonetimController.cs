using Kuafor_web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Kuafor_web.Models;

namespace Kuafor_web.Controllers
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





        