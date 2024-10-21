using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;

namespace KluczToSukcesDoKariery.Controllers
{
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
              return _context.Notatki != null ? 
                          View(await _context.Notatki.ToListAsync()) :
                          Problem("Entity set 'KluczToSukcesDoKarieryContext.Notatki'  is null.");
        }

        // GET: Notatkis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notatki == null)
            {
                return NotFound();
            }

            var notatki = await _context.Notatki
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notatki == null)
            {
                return NotFound();
            }

            return View(notatki);
        }

        // GET: Notatkis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notatkis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tytul,Tresc,DataDodania,UserId")] Notatki notatki)
        {
            if (ModelState.IsValid)
            {
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
            if (notatki == null)
            {
                return NotFound();
            }
            return View(notatki);
        }

        // POST: Notatkis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,Tresc,DataDodania,UserId")] Notatki notatki)
        {
            if (id != notatki.Id)
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

            var notatki = await _context.Notatki
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notatki == null)
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
            if (_context.Notatki == null)
            {
                return Problem("Entity set 'KluczToSukcesDoKarieryContext.Notatki'  is null.");
            }
            var notatki = await _context.Notatki.FindAsync(id);
            if (notatki != null)
            {
                _context.Notatki.Remove(notatki);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotatkiExists(int id)
        {
          return (_context.Notatki?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
