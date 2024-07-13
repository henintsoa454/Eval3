using Evaluation_3.Models.Entity;
using Evaluation_3.Models.Entity.Additional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Evaluation_3.Controllers
{
    public class ProprietaireController : Controller
    {
        private readonly mada_immoContext _mada_immoContext;
        private readonly ILogger<AdminController> _logger;

        public ProprietaireController(mada_immoContext mada_immoContext, ILogger<AdminController> logger)
        {
            _mada_immoContext = mada_immoContext;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> checkLogin(string numeroTel)
        {
            Console.WriteLine("Proprietaire: " + numeroTel);
            if (ModelState.IsValid)
            {
                var proprietaire = _mada_immoContext.Proprietaires.SingleOrDefault(p => p.Numerotel == numeroTel);
                if (proprietaire != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, proprietaire.Numerotel)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    
                    HttpContext.Session.SetInt32("ProprietaireId", proprietaire.Id);

                    return RedirectToAction("Home", "Proprietaire");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("ProprietaireId");
            return RedirectToAction("Login");
        }

        public IActionResult Home()
        {
            return View();
        }

        public async Task<IActionResult> ListeBien(int idproprietaire)
        {
            var proprietaire = await _mada_immoContext.Proprietaires
                .Include(p => p.Biens)
                .ThenInclude(b => b.IdtypeNavigation)
                .ThenInclude(t => t.Biens)
                .ThenInclude(b => b.IdregionNavigation)
                .ThenInclude(r => r.Biens)
                .ThenInclude(b => b.Photobiens)
                .ThenInclude(pb => pb.IdphotoNavigation)
                .Include(p => p.Biens)
                .ThenInclude(b => b.LocationIdbienNavigations)
                .FirstOrDefaultAsync(p => p.Id == idproprietaire);

            if (proprietaire == null)
            {
                return NotFound();
            }

            return View(proprietaire);
        }

        public IActionResult ChiffreAffaireIntervalle(int idproprietaire)
        {
            return View(idproprietaire);
        }

        public async Task<IActionResult> ChiffreAffaire(int idproprietaire, DateTime startDate, DateTime endDate)
        {
            var locations = await _mada_immoContext.Locations
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.IdtypeNavigation)
                .Include(l => l.IdclientNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(l => l.IdproprietaireNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.LocationIdbienNavigations)
                .Where(l => l.IdbienNavigation.IdproprietaireNavigation.Id == idproprietaire)
                .ToListAsync();

            List<RapportRevenue> rapportRevenues = new List<RapportRevenue>();

            foreach (var location in locations)
            {
                int lastyear = endDate.Year;
                int lastmonth = endDate.Month;
                for (int i = 0; i < location.Duree; i++)
                {
                    int year = location.Datedebut.Year + ((i - 1 + location.Datedebut.Month) / 12);
                    RapportRevenue rapportRevenu = new RapportRevenue();
                    rapportRevenu.Month = location.Datedebut.AddMonths(i).Month;
                    rapportRevenu.Year = year;
                    rapportRevenu.client = location.IdclientNavigation.Nom;
                    rapportRevenu.bien = location.IdbienNavigation.Nom;
                    rapportRevenu.ordreMois = i + 1;
                    rapportRevenu.Loyer = location.IdbienNavigation.Loyermensuel;
                    rapportRevenu.Commission = location.IdbienNavigation.IdtypeNavigation.Commission;
                    rapportRevenues.Add(rapportRevenu);
                }
            }

            for (int i = 0; i < rapportRevenues.Count; i++)
            {
                var rapportRevenu = rapportRevenues[i];

                if (rapportRevenu.ordreMois == 1)
                {
                    rapportRevenu.Loyer = rapportRevenu.Loyer * 2;
                    rapportRevenu.Commission = 50;
                }

                rapportRevenu.Revenue = (rapportRevenu.Loyer * rapportRevenu.Commission) / 100;
                startDate = new DateTime(startDate.Year, startDate.Month, 1);
                endDate = new DateTime(endDate.Year, endDate.Month, 1);
                DateTime daterapport = new DateTime(rapportRevenu.Year, rapportRevenu.Month, 1);
                if (daterapport < startDate || daterapport > endDate)
                {
                    Console.WriteLine(startDate.ToString() + "<" + daterapport.ToString() + ">" + endDate.ToString());
                    Console.WriteLine("Un supprimer" + rapportRevenu.client);
                    rapportRevenues.RemoveAt(i);
                    i--;
                }
            }

            return View(rapportRevenues);
        }

        public async Task<IActionResult> ChiffreAffaireFiltre(int idproprietaire)
        {
            var locations = await _mada_immoContext.Locations
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.IdtypeNavigation)
                .Include(l => l.IdclientNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(l => l.IdproprietaireNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.LocationIdbienNavigations)
                .Where(l => l.IdbienNavigation.IdproprietaireNavigation.Id == idproprietaire)
                .ToListAsync();

            List<RapportRevenue> rapportRevenues = new List<RapportRevenue>();

            foreach (var location in locations)
            {
                for (int i = 0; i < location.Duree; i++)
                {
                    int year = location.Datedebut.Year + ((i - 1 + location.Datedebut.Month) / 12);
                    RapportRevenue rapportRevenu = new RapportRevenue();
                    rapportRevenu.Month = location.Datedebut.AddMonths(i).Month;
                    rapportRevenu.Year = year;
                    rapportRevenu.client = location.IdclientNavigation.Nom;
                    rapportRevenu.bien = location.IdbienNavigation.Nom;
                    rapportRevenu.ordreMois = i + 1;
                    rapportRevenu.Loyer = location.IdbienNavigation.Loyermensuel;
                    rapportRevenu.Commission = location.IdbienNavigation.IdtypeNavigation.Commission;
                    rapportRevenues.Add(rapportRevenu);
                }
            }

            for (int i = 0; i < rapportRevenues.Count; i++)
            {
                var rapportRevenu = rapportRevenues[i];

                if (rapportRevenu.ordreMois == 1)
                {
                    rapportRevenu.Loyer = rapportRevenu.Loyer * 2;
                    rapportRevenu.Commission = 50;
                }

                rapportRevenu.Revenue = (rapportRevenu.Loyer * rapportRevenu.Commission) / 100;
            }
            return View(rapportRevenues);
        }

        [HttpGet]
        public async Task<IActionResult> DoFiltre(int idproprietaire, DateTime startDate, DateTime endDate)
        {
            var locations = await _mada_immoContext.Locations
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.IdtypeNavigation)
                .Include(l => l.IdclientNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(l => l.IdproprietaireNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.LocationIdbienNavigations)
                .Where(l => l.IdbienNavigation.IdproprietaireNavigation.Id == idproprietaire)
                .ToListAsync();

            List<RapportRevenue> rapportRevenues = new List<RapportRevenue>();

            foreach (var location in locations)
            {
                int lastyear = endDate.Year;
                int lastmonth = endDate.Month;
                for (int i = 0; i < location.Duree; i++)
                {
                    int year = location.Datedebut.Year + ((i - 1 + location.Datedebut.Month) / 12);
                    RapportRevenue rapportRevenu = new RapportRevenue();
                    rapportRevenu.Month = location.Datedebut.AddMonths(i).Month;
                    rapportRevenu.Year = year;
                    rapportRevenu.client = location.IdclientNavigation.Nom;
                    rapportRevenu.bien = location.IdbienNavigation.Nom;
                    rapportRevenu.ordreMois = i + 1;
                    rapportRevenu.Loyer = location.IdbienNavigation.Loyermensuel;
                    rapportRevenu.Commission = location.IdbienNavigation.IdtypeNavigation.Commission;
                    rapportRevenues.Add(rapportRevenu);
                }
            }

            for (int i = 0; i < rapportRevenues.Count; i++)
            {
                var rapportRevenu = rapportRevenues[i];

                if (rapportRevenu.ordreMois == 1)
                {
                    rapportRevenu.Loyer = rapportRevenu.Loyer * 2;
                    rapportRevenu.Commission = 50;
                }

                rapportRevenu.Revenue = (rapportRevenu.Loyer * rapportRevenu.Commission) / 100;
                startDate = new DateTime(startDate.Year, startDate.Month, 1);
                endDate = new DateTime(endDate.Year, endDate.Month, 1);
                DateTime daterapport = new DateTime(rapportRevenu.Year, rapportRevenu.Month, 1);
                if (daterapport < startDate || daterapport > endDate)
                {
                    Console.WriteLine(startDate.ToString() + "<" + daterapport.ToString() + ">" + endDate.ToString());
                    Console.WriteLine("Un supprimer" + rapportRevenu.client);
                    rapportRevenues.RemoveAt(i);
                    i--;
                }
            }
            return View("ChiffreAffaireFiltre", rapportRevenues);
        }
    }
}
