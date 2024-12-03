using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KluczToSukcesDoKariery.Controllers
{
    public class NewsController : Controller
    {
        private readonly KluczToSukcesDoKarieryContext _context;

        public NewsController(KluczToSukcesDoKarieryContext context)
        {
            _context = context;
        }

        // Widok listy aktualności (Index)
        [Authorize]
        public IActionResult Index()
        {
            var newsList = _context.News.ToList();  // Pobierz wszystkie aktualności z bazy danych
            return View(newsList);  // Zwróć listę do widoku
        }

        // Widok dodawania nowej aktualności (Create GET)
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();  // Przekaż pusty model do widoku
        }

        // Dodanie nowej aktualności (Create POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(News news)
        {
            if (ModelState.IsValid)
            {
                news.CreatedAt = DateTime.Now;  // Ustaw datę utworzenia
                _context.News.Add(news);  // Dodaj nową aktualność do bazy danych
                _context.SaveChanges();  // Zapisz zmiany w bazie
                return RedirectToAction(nameof(Index));  // Przekieruj do listy aktualności
            }
            return View(news);  // W przypadku błędów zwróć formularz z błędami
        }

        // Widok edytowania aktualności (Edit GET)
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id)
        {
            var news = _context.News.Find(id);  // Znajdź aktualność po id
            if (news == null)
            {
                return NotFound();  // Jeśli aktualność nie istnieje, zwróć błąd
            }
            return View(news);  // Przekaż aktualność do widoku edycji
        }

        // Edytowanie aktualności (Edit POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, News news)
        {
            if (id != news.Id)
            {
                return NotFound();  // Sprawdź, czy id w URL zgadza się z id aktualności
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);  // Zaktualizuj aktualność w bazie
                    _context.SaveChanges();  // Zapisz zmiany
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.News.Any(n => n.Id == id))
                    {
                        return NotFound();  // Jeśli aktualność nie istnieje, zwróć błąd
                    }
                    else
                    {
                        throw;  // Jeśli wystąpił inny błąd, rzuć wyjątek
                    }
                }
                return RedirectToAction(nameof(Index));  // Po zapisaniu zmiany, przekieruj do listy
            }
            return View(news);  // Jeśli wystąpiły błędy, zwróć formularz edycji
        }

        // Widok usuwania aktualności (Delete GET)
        public IActionResult Delete(int id)
        {
            var news = _context.News.Find(id);  // Znajdź aktualność po id
            if (news == null)
            {
                return NotFound();  // Jeśli aktualność nie istnieje, zwróć błąd
            }
            return View(news);  // Przekaż aktualność do widoku usuwania
        }

        // Usuwanie aktualności (Delete POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteConfirmed(int id)
        {
            var news = _context.News.Find(id);  // Znajdź aktualność po id
            if (news == null)
            {
                return NotFound();  // Jeśli aktualność nie istnieje, zwróć błąd
            }

            _context.News.Remove(news);  // Usuń aktualność z bazy danych
            _context.SaveChanges();  // Zapisz zmiany w bazie
            return RedirectToAction(nameof(Index));  // Po usunięciu przekieruj do listy aktualności
        }
    }
}
