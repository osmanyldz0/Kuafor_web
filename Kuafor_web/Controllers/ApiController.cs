using Kuafor_web.Models;
 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace berber4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : Controller
    {
        private readonly BerberDbContext _context;
        public ApiController(BerberDbContext context)
        {
            _context = context;
        }

        // Çalışanın izin gününü ve BerberId'sini döndüren API
        [HttpGet("GetIzinGunuByBerber/{id}")]
        public async Task<ActionResult<Api>> GetIzinGunuByBerber(int id)
        {
            // Berber (çalışan) için veritabanından gerekli bilgiyi alıyoruz
            var b = await _context.Berbers
                .FirstOrDefaultAsync(b => b.BerberId == id);

            if (b == null)
            {
                return NotFound("Berber bulunamadı");
            }

            // IzinGunu ve BerberId döndürüyoruz
            var result = new Api
            {
                BerberId = b.BerberId,
                IzinGunu = b.CalisilmayanGun // Burada izin günü, bugünün tarihi olarak dönecek
            };

            return Ok(result);
        }
    }
}
