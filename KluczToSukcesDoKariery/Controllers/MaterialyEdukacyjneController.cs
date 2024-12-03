using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KluczToSukcesDoKariery.Controllers
{
    [Authorize]
    public class MaterialyEdukacyjneController : Controller
    {
        private readonly KluczToSukcesDoKarieryContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public MaterialyEdukacyjneController(KluczToSukcesDoKarieryContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

        // Index - widok wszystkich materiałów edukacyjnych
        public IActionResult Index()
        {
            var uid = _userManager.GetUserId(User);

            ViewBag.materials = _context.MaterialEdu?.AsEnumerable().GroupJoin(
                _context.MaterialEduLike!.AsEnumerable(),
                m => m.Id,
                l => l.MaterialEduId,
                (m, lGroup) => new {
                    MaterialEdu = m,
                    Likes = lGroup.Count(),
                    UserLike = uid != null && lGroup.FirstOrDefault(l => l.UserId == uid) != null
                }
            ).ToList();
            return View();
        }

        // Create - dodawanie nowego materiału (dostępne tylko dla administratora)
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View(new MaterialEdu());
        }

        // POST: Create
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(MaterialEdu material)
        {
            if (ModelState.IsValid)
            {
                _context.MaterialEdu.Add(material);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(material);
        }

        // Edit - edytowanie materiału (dostępne tylko dla administratora)
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id)
        {
            var material = _context.MaterialEdu.FirstOrDefault(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        // POST: Edit
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id, MaterialEdu material)
        {
            if (id != material.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(material);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(material);
        }

        // Delete - usuwanie materiału (dostępne tylko dla administratora)
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            var material = _context.MaterialEdu.FirstOrDefault(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteConfirmed(int id)
        {
            var material = _context.MaterialEdu.FirstOrDefault(m => m.Id == id);
            if (material != null)
            {
                _context.MaterialEdu.Remove(material);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleLike(int id)
        {
            var material = _context.MaterialEdu?.FirstOrDefault(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }

            var uid = _userManager.GetUserId(User);

            var like = _context.MaterialEduLike?.FirstOrDefault(l => l.MaterialEduId == id && l.UserId == uid);

            if (like == null)
            {
                _context.Add(new MaterialEduLike(material.Id, uid));
                _context.SaveChanges();
                return Created("", null);
            } else
            {
                _context.Remove(like);
                _context.SaveChanges();
                return NoContent();
			}
        }
    }
}