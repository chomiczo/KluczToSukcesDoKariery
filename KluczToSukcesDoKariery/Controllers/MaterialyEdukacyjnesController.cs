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
    public class MaterialyEdukacyjnesController : Controller
    {
        private readonly KluczToSukcesDoKarieryContext _context;

        public MaterialyEdukacyjnesController(KluczToSukcesDoKarieryContext context)
        {
            _context = context;
        }

        // GET: MaterialyEdukacyjnes
        public async Task<IActionResult> Index()
        {
              return _context.MaterialyEdukacyjne != null ? 
                          View(await _context.MaterialyEdukacyjne.ToListAsync()) :
                          Problem("Entity set 'KluczToSukcesDoKarieryContext.MaterialyEdukacyjne'  is null.");
        }

        // GET: MaterialyEdukacyjnes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MaterialyEdukacyjne == null)
            {
                return NotFound();
            }

            var materialyEdukacyjne = await _context.MaterialyEdukacyjne
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialyEdukacyjne == null)
            {
                return NotFound();
            }

            return View(materialyEdukacyjne);
        }

        // GET: MaterialyEdukacyjnes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaterialyEdukacyjnes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tytul,Opis,Autor,DataDodania,Url")] MaterialyEdukacyjne materialyEdukacyjne)
        {
            if (ModelState.IsValid)
            {
                _context.Add(materialyEdukacyjne);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materialyEdukacyjne);
        }

        // GET: MaterialyEdukacyjnes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MaterialyEdukacyjne == null)
            {
                return NotFound();
            }

            var materialyEdukacyjne = await _context.MaterialyEdukacyjne.FindAsync(id);
            if (materialyEdukacyjne == null)
            {
                return NotFound();
            }
            return View(materialyEdukacyjne);
        }

        // POST: MaterialyEdukacyjnes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,Opis,Autor,DataDodania,Url")] MaterialyEdukacyjne materialyEdukacyjne)
        {
            if (id != materialyEdukacyjne.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materialyEdukacyjne);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialyEdukacyjneExists(materialyEdukacyjne.Id))
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
            return View(materialyEdukacyjne);
        }

        // GET: MaterialyEdukacyjnes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MaterialyEdukacyjne == null)
            {
                return NotFound();
            }

            var materialyEdukacyjne = await _context.MaterialyEdukacyjne
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materialyEdukacyjne == null)
            {
                return NotFound();
            }

            return View(materialyEdukacyjne);
        }

        // POST: MaterialyEdukacyjnes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MaterialyEdukacyjne == null)
            {
                return Problem("Entity set 'KluczToSukcesDoKarieryContext.MaterialyEdukacyjne'  is null.");
            }
            var materialyEdukacyjne = await _context.MaterialyEdukacyjne.FindAsync(id);
            if (materialyEdukacyjne != null)
            {
                _context.MaterialyEdukacyjne.Remove(materialyEdukacyjne);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialyEdukacyjneExists(int id)
        {
          return (_context.MaterialyEdukacyjne?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
