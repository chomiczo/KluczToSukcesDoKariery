using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace KluczToSukcesDoKariery.Controllers
{
    [Authorize]
    public class CustomerModelsController : Controller
    {
        private readonly KluczToSukcesDoKarieryContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CustomerModelsController> _logger;
        public CustomerModelsController(KluczToSukcesDoKarieryContext context, UserManager<IdentityUser> userManager, ILogger<CustomerModelsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: CustomerModels
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            return _context.CustomerModel != null ?
                          View(await _context.CustomerModel.Where(m => m.UserId == user.Id).ToListAsync()) :
                          Problem("Entity set 'KluczToSukcesDoKarieryContext.CustomerModel' is null.");


        }

        // GET: CustomerModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CustomerModel == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // GET: CustomerModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email")] CustomerModel customerModel)
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            customerModel.UserId = user.Id;

            var existingCustomer = _context.CustomerModel.FirstOrDefault(c => c.UserId == user.Id);


            if (existingCustomer != null)
            {
                // Możesz obsłużyć przypadek, gdy użytkownik ma już utworzone konto klienta
                ModelState.AddModelError(string.Empty, "Nie możesz wypełnić ponownie tego formularza, ponieważ masz już istniejące konto użytkownika! Formularz można wypełnić tylko raz na jedno konto!");
            }

            if (ModelState.IsValid)
            {
                if (existingCustomer == null)
                {
                    _context.Add(customerModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(customerModel);
        }

        // GET: CustomerModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CustomerModel == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel.FindAsync(id);
            if (customerModel == null)
            {
                return NotFound();
            }
            return View(customerModel);
        }

        // POST: CustomerModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email")] CustomerModel customerModel)
        {
            if (id != customerModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerModelExists(customerModel.Id))
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
            return View(customerModel);
        }

        // GET: CustomerModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CustomerModel == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // POST: CustomerModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CustomerModel == null)
            {
                return Problem("Entity set 'KluczToSukcesDoKarieryContext.CustomerModel'  is null.");
            }
            var customerModel = await _context.CustomerModel.FindAsync(id);
            if (customerModel != null)
            {
                _context.CustomerModel.Remove(customerModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerModelExists(int id)
        {
            return (_context.CustomerModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
