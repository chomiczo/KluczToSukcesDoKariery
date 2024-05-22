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
    public class QuizyZawodowesController : Controller
    {
        private readonly KluczToSukcesDoKarieryContext _context;

        public QuizyZawodowesController(KluczToSukcesDoKarieryContext context)
        {
            _context = context;
        }

        // GET: QuizyZawodowes
        public async Task<IActionResult> Index()
        {
              return _context.QuizyZawodowe != null ? 
                          View(await _context.QuizyZawodowe.ToListAsync()) :
                          Problem("Entity set 'KluczToSukcesDoKarieryContext.QuizyZawodowe'  is null.");
        }

        // GET: QuizyZawodowes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.QuizyZawodowe == null)
            {
                return NotFound();
            }

            var quizyZawodowe = await _context.QuizyZawodowe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizyZawodowe == null)
            {
                return NotFound();
            }

            return View(quizyZawodowe);
        }

        // GET: QuizyZawodowes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QuizyZawodowes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tytul,Opis,DataUtworzenia,DataModyfikacji")] QuizyZawodowe quizyZawodowe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quizyZawodowe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quizyZawodowe);
        }

        // GET: QuizyZawodowes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.QuizyZawodowe == null)
            {
                return NotFound();
            }

            var quizyZawodowe = await _context.QuizyZawodowe.FindAsync(id);
            if (quizyZawodowe == null)
            {
                return NotFound();
            }
            return View(quizyZawodowe);
        }

        // POST: QuizyZawodowes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,Opis,DataUtworzenia,DataModyfikacji")] QuizyZawodowe quizyZawodowe)
        {
            if (id != quizyZawodowe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quizyZawodowe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizyZawodoweExists(quizyZawodowe.Id))
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
            return View(quizyZawodowe);
        }

        // GET: QuizyZawodowes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.QuizyZawodowe == null)
            {
                return NotFound();
            }

            var quizyZawodowe = await _context.QuizyZawodowe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizyZawodowe == null)
            {
                return NotFound();
            }

            return View(quizyZawodowe);
        }

        // POST: QuizyZawodowes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.QuizyZawodowe == null)
            {
                return Problem("Entity set 'KluczToSukcesDoKarieryContext.QuizyZawodowe'  is null.");
            }
            var quizyZawodowe = await _context.QuizyZawodowe.FindAsync(id);
            if (quizyZawodowe != null)
            {
                _context.QuizyZawodowe.Remove(quizyZawodowe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizyZawodoweExists(int id)
        {
          return (_context.QuizyZawodowe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
