using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RS1_Ispit_asp.net_core.EF;
using RS1_Ispit_asp.net_core.EntityModels;
using RS1_Ispit_asp.net_core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.Controllers
{
    public class TakmicenjeController : Controller
    {
        private MojContext _context;

        public TakmicenjeController(MojContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new TakmicenjeIndexVM
            {
                Skole = _context.Skola.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Naziv
                }).ToList(),
                Predmeti = _context.Predmet.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Naziv} (razred {x.Razred})"
                }).OrderBy(x => x.Text).ToList()
            };
            model.Zapisi = _context.Takmicenje
                .Include(x => x.Skola)
                .Include(x => x.Predmet).Select(x => new TakmicenjeIndexVM.Zapis
            {
                Takmicenje = x,
            }).ToList();

            foreach (var item in model.Zapisi)
            {
                item.NajboljiUcesnik = GetNajbolji(item.Takmicenje.Id);
            }

            return View(model);
        }

        public IActionResult Prikazi(int SkolaFilterId, int PredmetFilterId)
        {
            var model = _context.Takmicenje.Where(x => x.SkolaId == SkolaFilterId
            && x.PredmetId == PredmetFilterId)
                .Include(x => x.Skola)
                .Include(x => x.Predmet)
                .Select(x => new TakmicenjeIndexVM.Zapis { 
                Takmicenje = x
            }).ToList();
            foreach (var item in model)
            {
                item.NajboljiUcesnik = GetNajbolji(item.Takmicenje.Id);
            }
            return View("Tabela", model);
        }

        public IActionResult Dodaj(int SkolaFilterId, int PredmetFilterId)
        {
            var model = new TakmicenjeDodajVM
            {
                Skola = _context.Skola.Find(SkolaFilterId),
                Predmet = _context.Predmet.Find(PredmetFilterId),
                Datum = DateTime.Now
            };
            return View(model);
        }

        public IActionResult Snimi(TakmicenjeDodajVM model)
        {
            var takmicenje = new Takmicenje
            {
                SkolaId = model.Skola.Id,
                PredmetId = model.Predmet.Id,
                Datum = model.Datum,
                Zakljucano = false
            };
            _context.Takmicenje.Add(takmicenje);
            _context.SaveChanges();

            var ucesnici = _context.DodjeljenPredmet.Where(x => x.PredmetId == model.Predmet.Id
            && x.ZakljucnoKrajGodine > 3).Select(x => new TakmicenjeUcesnik 
            {
                TakmicenjeId = takmicenje.Id,
                OdjeljenjeStavkaId = x.OdjeljenjeStavkaId,
                Pristupio = false,
                Bodovi = null
            });

            _context.TakmicenjeUcesnik.AddRange(ucesnici);
            _context.SaveChanges();

            return Redirect("Index");
        }

        public IActionResult Rezultati(int Id)
        {
            var model = new TakmicenjeRezultatiVM
            {
                Takmicenje = _context.Takmicenje.Include(x => x.Skola)
                .Include(x => x.Predmet).Where(x => x.Id == Id).FirstOrDefault(),
                Ucesnici = _context.TakmicenjeUcesnik.Include(x => x.OdjeljenjeStavka)
                .Include(x => x.OdjeljenjeStavka.Odjeljenje)
                .Where(x => x.TakmicenjeId == Id).ToList()
            };
            return View(model);
        }

        public void Zakljucaj(int Id)
        {
            var takmicenje = _context.Takmicenje.Find(Id);
            takmicenje.Zakljucano = true;
            _context.Entry(takmicenje).State = EntityState.Modified;
            _context.SaveChanges();
            return;
        }

        public void Prisustvo(int Id)
        {
            var ucesnik = _context.TakmicenjeUcesnik.Find(Id);
            if (_context.Takmicenje.Find(ucesnik.TakmicenjeId).Zakljucano)
                return;
            ucesnik.Pristupio = !ucesnik.Pristupio;
            if (!ucesnik.Pristupio)
                ucesnik.Bodovi = null;
            _context.Entry(ucesnik).State = EntityState.Modified;
            _context.SaveChanges();
            return;
        }

        public IActionResult UrediUcesnika(int Id, int TakmicenjeId)
        {
            var ucesnik = _context.TakmicenjeUcesnik
                .Include(x => x.OdjeljenjeStavka.Odjeljenje)
                .Include(x => x.OdjeljenjeStavka.Ucenik).Where(x => x.Id == Id).FirstOrDefault();
            var model = new TakmicenjeUcesnikVM
            {
                UcesnikId = Id,
                Ucesnik = ucesnik.OdjeljenjeStavka.Odjeljenje.Oznaka + " - " +
                ucesnik.OdjeljenjeStavka.Ucenik.ImePrezime + " - " +
                ucesnik.OdjeljenjeStavka.BrojUDnevniku,
                Bodovi = ucesnik.Bodovi,
                Uredi = true,
                TakmicenjeId = TakmicenjeId
            };
            return View("Ucesnik", model);
        }

        public IActionResult DodajUcesnika(int TakmicenjeId)
        {
            var ucesnik = new TakmicenjeUcesnikVM {
                Ucesnici = _context.OdjeljenjeStavka.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Odjeljenje.Oznaka + " - " + x.Ucenik.ImePrezime + " - "
                    + x.BrojUDnevniku
                }).ToList(),
                TakmicenjeId = TakmicenjeId
            };
            return View("Ucesnik", ucesnik);
        }

        public IActionResult SnimiUcesnika(TakmicenjeUcesnikVM model)
        {
            if(_context.Takmicenje.Find(model.TakmicenjeId).Zakljucano)
                return RedirectToAction("Rezultati", new { Id = model.TakmicenjeId });

            if (model.Uredi)
            {
                var ucesnik = _context.TakmicenjeUcesnik.Find(model.UcesnikId);
                ucesnik.Bodovi = model.Bodovi;
                ucesnik.Pristupio = true;
                _context.Entry(ucesnik).State = EntityState.Modified;
            }
            else
            {
                var ucesnik = new TakmicenjeUcesnik
                {
                    OdjeljenjeStavkaId = model.UcesnikId,
                    Pristupio = true,
                    Bodovi = model.Bodovi,
                    TakmicenjeId = model.TakmicenjeId
                };
                _context.TakmicenjeUcesnik.Add(ucesnik);
            }
            _context.SaveChanges();
            return RedirectToAction("Rezultati", new { Id = model.TakmicenjeId });
        }

        public void NoviBodovi(int Id, int bodovi)
        {
            var ucesnik = _context.TakmicenjeUcesnik.Find(Id);

            if (_context.Takmicenje.Find(ucesnik.TakmicenjeId).Zakljucano)
                return;

            ucesnik.Bodovi = bodovi;
            if (ucesnik.Bodovi != null)
                ucesnik.Pristupio = true;
            _context.Entry(ucesnik).State = EntityState.Modified;
            _context.SaveChanges();
            return;
        }

        private string GetNajbolji(int id)
        {
            var ucesnik = _context.TakmicenjeUcesnik.Where(x => x.TakmicenjeId == id)
                .Include(x => x.OdjeljenjeStavka.Odjeljenje).Include(x => x.OdjeljenjeStavka.Ucenik)
                .OrderByDescending(x => x.Bodovi).FirstOrDefault();
            if (ucesnik == null)
                return "Nema ucesnika";
            else
            {
                return ucesnik.OdjeljenjeStavka.Odjeljenje.Oznaka + " - "
                    + ucesnik.OdjeljenjeStavka.Ucenik.ImePrezime;
            }
        }
    }
}
