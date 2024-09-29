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

        // GET: QuizyZawodowes/QuizWstepny
        public IActionResult QuizWstepny()
        {
            return View();
        }

// POST: QuizyZawodowes/ProcessQuizWstepny
[HttpPost]
public IActionResult ProcessQuizWstepny(IFormCollection form)
{
    // Extract the quiz answers from the form
    var workType = form["workType"];
    var environment = form["environment"];
    var teamwork = form["teamwork"];
    var interests = form["interests"];
    var workHours = form["workHours"];
    var skills = form["skills"]; // This will be a collection
    var taskType = form["taskType"];
    var employmentType = form["employmentType"];
    var values = form["values"]; // This will also be a collection
    var teamRole = form["teamRole"];

    // Process the quiz answers (e.g., save to database, analyze, etc.)

    // Redirect to a result page or return the result directly
    return RedirectToAction("QuizResult", new 
    { 
        workType, 
        environment, 
        teamwork, 
        interests, 
        workHours,
        skills = skills.ToList(), // Convert to a list if necessary
        taskType,
        employmentType,
        values = values.ToList(), // Convert to a list if necessary
        teamRole 
    });
}


        // GET: QuizyZawodowes/QuizResult
        public IActionResult QuizResult(string workType, string environment, string teamwork, string interests, string workHours, List<string> skills, string taskType, string employmentType, List<string> values, string teamRole)
        {
            ViewBag.WorkType = workType;
            ViewBag.Environment = environment;
            ViewBag.Teamwork = teamwork;
            ViewBag.Interests = interests;
            ViewBag.WorkHours = workHours;
            ViewBag.Skills = skills;
            ViewBag.TaskType = taskType;
            ViewBag.EmploymentType = employmentType;
            ViewBag.Values = values;
            ViewBag.TeamRole = teamRole;

            return View();
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
