using Evaluation_3.Models.Entity;
using Evaluation_3.Models.Entity.Additional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Evaluation_3.Controllers
{
    public class AdminController : Controller
    {
        private readonly mada_immoContext _mada_immoContext;
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string _csvFolder = "upload/csv";

        public AdminController(mada_immoContext mada_immoContext, ILogger<AdminController> logger, IWebHostEnvironment hostEnvironment)
        {
            _mada_immoContext = mada_immoContext;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> checkLogin(string login, string password)
        {
            Console.WriteLine("Admin: " + login);
            if (ModelState.IsValid)
            {
                var admin = _mada_immoContext.Admins.SingleOrDefault(a => a.Login == login && a.Password == password);
                if (admin != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.Login)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    HttpContext.Session.SetInt32("AdminId", admin.Id);

                    return RedirectToAction("DashBoard", "Admin");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View("Login");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("Login");
        }

        public IActionResult DashBoard()
        {
            return View();
        }


        public IActionResult ReinitialisationDonnée()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reinitialiser()
        {
            try
            {
                await _mada_immoContext.Database.ExecuteSqlRawAsync("SET CONSTRAINTS ALL DEFERRED;");

                _mada_immoContext.Proprietaires.RemoveRange(_mada_immoContext.Proprietaires);
                _mada_immoContext.Clients.RemoveRange(_mada_immoContext.Clients);
                _mada_immoContext.Biens.RemoveRange(_mada_immoContext.Biens);
                _mada_immoContext.Locations.RemoveRange(_mada_immoContext.Locations);
                _mada_immoContext.Photobiens.RemoveRange(_mada_immoContext.Photobiens);
                _mada_immoContext.Regions.RemoveRange(_mada_immoContext.Regions);
                _mada_immoContext.Typebiens.RemoveRange(_mada_immoContext.Typebiens);

                await _mada_immoContext.SaveChangesAsync();

                await _mada_immoContext.Database.ExecuteSqlRawAsync("SET CONSTRAINTS ALL IMMEDIATE;");

                return RedirectToAction("DashBoard", "Admin");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult ChiffreAffaireGainIntervalle()
        {
            return View();
        }

        public async Task<IActionResult> ChiffreAffaireGain(DateTime startDate, DateTime endDate)
        {
            var locations = await _mada_immoContext.Locations
                .Include(l => l.IdclientNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.IdtypeNavigation)
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

        public IActionResult FormImportDonnée()
        {
            return View();
        }

        public async Task<IActionResult> FormAjoutLocation()
        {
            var biens = await _mada_immoContext.Biens
                .Include(b => b.IdproprietaireNavigation)
                .Include(b => b.IdregionNavigation)
                .Include(b => b.IdtypeNavigation)
                .ToListAsync();

            var clients = await _mada_immoContext.Clients.ToListAsync();

            var model = new Tuple<List<Bien>, List<Client>>(biens, clients);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertionLocation(int IdClient, int IdBien, int Duree, DateTime DateDebut)
        {
            if (ModelState.IsValid)
            {
                var bien = await _mada_immoContext.Biens
                    .Include(b => b.IdtypeNavigation)
                    .ThenInclude(t => t.Biens)
                    .Include(b => b.IdregionNavigation)
                    .ThenInclude(r => r.Biens)
                    .ThenInclude(b => b.Photobiens)
                    .ThenInclude(pb => pb.IdphotoNavigation)
                    .Include(b => b.LocationIdbienNavigations)
                    .FirstOrDefaultAsync(b => b.Id == IdBien);

                if (DateDebut <= bien.getDateDisponibilité() || DateDebut < DateTime.Now)
                {
                    TempData["ErrorMessage"] = "La date de début doit être après la date de disponibilité du bien ou une date qui n'est pas du passé.";

                    var bi = await _mada_immoContext.Biens
                        .Include(b => b.IdproprietaireNavigation)
                        .Include(b => b.IdregionNavigation)
                        .Include(b => b.IdtypeNavigation)
                        .ToListAsync();

                    var cls = await _mada_immoContext.Clients.ToListAsync();

                    var ms = new Tuple<List<Bien>, List<Client>>(bi, cls);
                    return View("FormAjoutLocation", ms);
                }

                var location = new Location
                {
                    Referencebien = bien.Reference,
                    Idclient = IdClient,
                    Idbien = IdBien,
                    Duree = Duree,
                    Datedebut = DateDebut,
                    Ispayed = 0
                };

                _mada_immoContext.Locations.Add(location);
                await _mada_immoContext.SaveChangesAsync();

                return RedirectToAction("DashBoard");
            }

            TempData["ErrorMessage"] = "Erreur lors de la validation du formulaire.";

            var b = await _mada_immoContext.Biens
                .Include(b => b.IdproprietaireNavigation)
                .Include(b => b.IdregionNavigation)
                .Include(b => b.IdtypeNavigation)
                .ToListAsync();

            var cs = await _mada_immoContext.Clients.ToListAsync();

            var m = new Tuple<List<Bien>, List<Client>>(b, cs);
            return View("FormAjoutLocation", m);
        }


        public async Task<IActionResult> InsertionDonnee(IFormFile commissions, IFormFile biens, IFormFile locations)
        {
            CsvcommissionFunction csvcommission = new CsvcommissionFunction();
            CsvbienFunction csvbien = new CsvbienFunction();
            CsvLocationFunction csvlocation = new CsvLocationFunction();
            await csvcommission.DispatchToTableAsync(_mada_immoContext, _hostEnvironment, _csvFolder, commissions);
            await csvbien.DispatchToTableAsync(_mada_immoContext, _hostEnvironment, _csvFolder, biens);
            await csvlocation.DispatchToTableAsync(_mada_immoContext, _hostEnvironment, _csvFolder, locations);

            var photos = await _mada_immoContext.Photos.ToListAsync();

            var bs = await _mada_immoContext.Biens.ToListAsync();

            foreach (var bien in bs)
            {
                foreach (var photo in photos)
                {
                    var photoBien = new Photobien
                    {
                        Idbien = bien.Id,
                        Idphoto = photo.Id
                    };

                    _mada_immoContext.Photobiens.Add(photoBien);
                }
            }

            await _mada_immoContext.SaveChangesAsync();

            return RedirectToAction("DashBoard", "Admin");
        }
        public async Task<IActionResult> ChiffreAffaireGainFiltre()
        {
            var locations = await _mada_immoContext.Locations
                    .Include(l => l.IdclientNavigation)
                    .Include(l => l.IdbienNavigation)
                    .ThenInclude(b => b.IdtypeNavigation)
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
        public async Task<IActionResult> DoFiltre(DateTime startDate, DateTime endDate)
        {
            var locations = await _mada_immoContext.Locations
                .Include(l => l.IdclientNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.IdtypeNavigation)
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
            return View("ChiffreAffaireGainFiltre", rapportRevenues);
        }

        public async Task<IActionResult> ListeLocation()
        {
            var locations = await _mada_immoContext.Locations
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.IdtypeNavigation)
                .Include(l => l.IdclientNavigation)
                .ToListAsync();

            return View(locations);
        }

        public async Task<IActionResult> DetailListeLocation(int idlocation)
        {
            var locations = await _mada_immoContext.Locations
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.IdtypeNavigation)
                .Include(l => l.IdclientNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(l => l.IdproprietaireNavigation)
                .Include(l => l.IdbienNavigation)
                .ThenInclude(b => b.LocationIdbienNavigations)
                .Where(l => l.Id == idlocation)
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
    }
}
