using Kuafor_web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace berber4.Controllers
{
    public class GirisController : Controller
    {
        public IActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GirisYap(Kullanici k,string ReturnUrl)
        {
            BerberDbContext db = new BerberDbContext();
            var kullanici = db.Kullanicis.FirstOrDefault(kul => kul.Eposta == k.Eposta && MD5Sifrele( k.Parola) == kul.Parola && kul.Silindi == false && kul.Aktif == true);
           
            if(kullanici!=null)

            {

                string yetki = (bool)kullanici.Yetki ? "Yonetici" : "Uye" ;
                var talepler = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email,kullanici.Eposta.ToString()),
                     new Claim(ClaimTypes.Role,yetki),
                     new Claim(ClaimTypes.NameIdentifier,kullanici.KullaniciId.ToString())

                };
                ClaimsIdentity kimlik= new ClaimsIdentity(talepler,"Login");
                ClaimsPrincipal kural = new ClaimsPrincipal(kimlik);
                await HttpContext.SignInAsync(kural);
                if(!string.IsNullOrEmpty(ReturnUrl))
                {
                    
              
                    return Redirect(ReturnUrl);

                }
                else
                {

                    if ((bool)kullanici.Yetki)
                    {
                        return Redirect("/Home/Anasayfa");

                    }
                    else
                    {
                        return Redirect("/Home/Anasayfa");
                    }










                   
                }

            }
            ViewData["ErrorMessage"] = "Kayıtlı kullanıcı bulunamadı bilgilerinizi kontrol ediniz !!!";
           return View();
            

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



      
        public async Task<IActionResult>   CikisYap()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Anasayfa", "Home");
        }










    }
}
