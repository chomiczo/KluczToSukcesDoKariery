using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;

namespace KluczToSukcesDoKariery.Controllers
{
    [Authorize]
    public class NotatkisController : Controller
    {
        private readonly KluczToSukcesDoKarieryContext _context;

        public NotatkisController(KluczToSukcesDoKarieryContext context)
        {
            _context = context;
        }

        // GET: Notatkis
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name; // Pobierz ID zalogowanego użytkownika
            var userNotes = await _context.Notatki
                .Where(n => n.UserId == userId) // Filtrowanie notatek na podstawie UserId
                .ToListAsync();
            return View(userNotes);
        }

        // GET: Notatkis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notatki == null)
            {
                return NotFound();
            }

            var notatki = await _context.Notatki.FirstOrDefaultAsync(m => m.Id == id);

            // Sprawdzenie, czy użytkownik jest właścicielem notatki
            if (notatki == null || notatki.UserId != User.Identity.Name)
            {
                return NotFound();
            }

            return View(notatki);
        }

        // GET: Notatkis/Create
        [HttpGet]
        public IActionResult Create()
        {
            var notatki = new Notatki
            {
                UserId = User.Identity.Name, // Automatyczne przypisanie UserId
                DataDodania = DateTime.Now
                
            };

            return View(notatki);
        }


        // POST: Notatkis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tytul,Tresc,DataDodania,UserId")] Notatki notatki)
        {
            // Sprawdzenie, czy użytkownik już posiada notatkę o tym samym tytule
            var userId = User.Identity.Name;
            bool noteExists = await _context.Notatki
                .AnyAsync(n => n.Tytul == notatki.Tytul && n.UserId == userId);

            if (noteExists)
            {
                ModelState.AddModelError("Tytul", "Notatka o takim tytule już istnieje.");
                notatki.DataDodania = DateTime.Now;
                return View(notatki); // Zwróć widok, jeśli tytuł już istnieje
            }


            if (ModelState.IsValid)
            {
                notatki.UserId = userId; // Przypisanie UserId
                notatki.DataDodania = DateTime.Now; // Ustawienie aktualnej daty
                _context.Add(notatki);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notatki);
        }

        // GET: Notatkis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Notatki == null)
            {
                return NotFound();
            }

            var notatki = await _context.Notatki.FindAsync(id);

            // Sprawdzenie, czy użytkownik jest właścicielem notatki
            if (notatki == null || notatki.UserId != User.Identity.Name)
            {
                return NotFound();
            }

            return View(notatki);
        }

        // POST: Notatkis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,Tresc,DataDodania,UserId")] Notatki notatki)
        {
            if (id != notatki.Id) // Sprawdzenie id
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notatki);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotatkiExists(notatki.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(notatki);
        }

        // GET: Notatkis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Notatki == null)
            {
                return NotFound();
            }

            var notatki = await _context.Notatki.FirstOrDefaultAsync(m => m.Id == id);

            // Sprawdzenie, czy użytkownik jest właścicielem notatki
            if (notatki == null || notatki.UserId != User.Identity.Name)
            {
                return NotFound();
            }

            return View(notatki);
        }

        // POST: Notatkis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notatki = await _context.Notatki.FindAsync(id);

            // Sprawdzenie, czy użytkownik jest właścicielem notatki
            if (notatki != null && notatki.UserId == User.Identity.Name)
            {
                _context.Notatki.Remove(notatki);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NotatkiExists(int id)
        {
            return (_context.Notatki?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}