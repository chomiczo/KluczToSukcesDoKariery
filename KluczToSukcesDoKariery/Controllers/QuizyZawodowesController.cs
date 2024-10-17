using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using SQLitePCL;
using NuGet.ProjectModel;

namespace KluczToSukcesDoKariery.Controllers
{
	public class QuizyZawodowesController : Controller
	{
		private readonly KluczToSukcesDoKarieryContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		private CustomerModel? GetCustomer()
		{
			if (User.Identity?.IsAuthenticated ?? false)
			{
				var uid = _userManager.GetUserId(User);
				return _context.CustomerModel?.ToList().FirstOrDefault(c => c.UserId == uid, null);
			}
			return null;
		}

		public QuizyZawodowesController(KluczToSukcesDoKarieryContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: QuizyZawodowes/QuizSpecjalnosciowy
		//[Route("QuizSpecjalnosciowy", Order = 1)]
		public IActionResult QuizSpecjalnosciowy(string? selectedJob)
		{
			var customer = GetCustomer();
			if (selectedJob == null && customer != null)
			{
				selectedJob = customer?.Job;


			}

			var quiz = _context.QuizyZawodowe?.Include("Pytania.Odpowiedzi")?.FirstOrDefault(q => q.Tytul == selectedJob);
			ViewBag.SelectedJob = selectedJob;
			ViewBag.Quiz = quiz;

			var helpAnswerIds = new List<int>();
			foreach (var pyt in quiz.Pytania)
			{
				if (pyt.Punktacja > 2)
				{

					var falseAnswers = pyt.Odpowiedzi?.Where(o => o.Poprawna == false).ToList();
					var r = new Random();
					falseAnswers?.RemoveAt(r.Next(falseAnswers.Count));
					foreach (var odp in falseAnswers)
					{
						helpAnswerIds.Add(odp.Id);
					}
				}
			}

			ViewBag.HelpAnswerIds = helpAnswerIds;


			return View();
		}

		[HttpPost]
		public IActionResult ChooseJob(string selectedJob)
		{
			var customer = GetCustomer();
			if (customer != null)
			{
				customer.Job = selectedJob;
				_context.SaveChanges();
			}

			return RedirectToAction("QuizSpecjalnosciowy", "QuizyZawodowes", new { selectedJob });
		}

		[HttpPost]
		public IActionResult QuizSpecjalnosciowyResult(IFormCollection form)
		{
			var quiz = _context.QuizyZawodowe?.Include("Pytania.Odpowiedzi").Single(
				q => q.Id == int.Parse(form["quiz-id"]));

			var customer = GetCustomer();
			var quizResult = _context.QuizyZawodoweWynik?.Include("Quiz").FirstOrDefault(qr => qr.Quiz == quiz && qr.Customer == customer);

			if (quizResult == null)
			{
				quizResult = new QuizyZawodoweWynik();
			}

			quizResult.Quiz = quiz;
			var userAnswers = quiz?.Pytania?.Join(
					form,
					p => $"pyt-{p.Id}",
					f => f.Key,
					(p, f) => new {
						p.Tekst,
						p.Punktacja,
						KoloUzyte = form[$"help-{p.Id}"] == "True",
						Poprawna = p.Odpowiedzi?.First(o => o.Id == int.Parse(f.Value)).Poprawna ?? false
					});
			quizResult.Wynik = userAnswers?.Where(x=>x.Poprawna).Sum(x => x.KoloUzyte ? x.Punktacja - 1 : x.Punktacja) ?? 0;

			if (customer != null && customer.UserId != null)
			{
				quizResult.Customer = customer;
				var task = _userManager.GetUserAsync(User);
				task.Wait();
				quizResult.User = task.Result;
				_context.QuizyZawodoweWynik?.Update(quizResult);
				_context.SaveChanges();
			}

			ViewBag.Answers = userAnswers;
			ViewBag.Total = quiz?.Pytania?.Sum(p => p.Punktacja);
			ViewBag.QuizResult = quizResult;
			return View();
		}

		public IActionResult Wynik()
		{
			var customer = GetCustomer();		

			if  (customer != null)
			{
				var quiz = _context.QuizyZawodowe?.Include("Pytania.Odpowiedzi").FirstOrDefault(q => q.Tytul == customer.Job);
				if (quiz != null)
				{
					try
					{
                        var quizResult = _context.QuizyZawodoweWynik?.First(qr => qr.QuizId == quiz.Id && qr.Customer == customer);

						ViewBag.QuizResult = quizResult;
                        ViewBag.Total = quiz?.Pytania?.Sum(p => p.Punktacja);
						return View("QuizSpecjalnosciowyResult"); 
					} catch
					{

					}
				}
			}
			return RedirectToAction("QuizSpecjalnosciowy");
		}

		// GET: QuizyZawodowes/QuizWstepny
		public IActionResult QuizWstepny()
		{
			if (User.Identity?.IsAuthenticated ?? false)
			{
				var uid = _userManager.GetUserId(User);

				try
				{
                    var qr = _context.QuizResults?.Single(q => q.UserId == uid);
					return RedirectToAction("QuizResult", qr);

				}
				catch
				{


				}
			}



			return View();
		}

		// POST: QuizyZawodowes/ProcessQuizWstepny
		[HttpPost]
		public IActionResult ProcessQuizWstepny(IFormCollection form)
		{
			Models.QuizResult quizResult;

			// Process the quiz answers (e.g., save to database, analyze, etc.)
			if (User.Identity?.IsAuthenticated ?? false)
			{
				using (var context = new KluczToSukcesDoKarieryContext(
					new DbContextOptions<KluczToSukcesDoKarieryContext>()))
				{
					var uid = _userManager.GetUserId(User);
					var task = _userManager.GetUserAsync(User);
					task.Wait();
					var user = task.Result;
					quizResult = new Models.QuizResult(form, user);
					_context.QuizResults?.Add(quizResult);
					_context.SaveChanges();

				}
			}
			else
			{
				quizResult = new Models.QuizResult(form);
			}

			// Redirect to a result page or return the result directly
			return RedirectToAction("QuizResult", quizResult);
		}

		// GET: QuizyZawodowes/QuizResult
		public IActionResult QuizResult(QuizResult quizResult)
		{
			ViewBag.QuizResult = quizResult;

			// Logika dopasowania zawodów
			var recommendedJobs = GetRecommendedJobs(quizResult);

			ViewBag.RecommendedJobs = recommendedJobs;

			return View();
		}

		[HttpPost]
		public IActionResult QuizAgain()
		{
			var customer = GetCustomer();
			var result = _context.QuizResults?.FirstOrDefault(qr => qr.UserId == customer.UserId);
			if (result != null)
			{
				_context.QuizResults?.Remove(result);
				_context.SaveChanges();
			}

			return RedirectToAction("QuizWstepny");
		}

		private List<string> GetRecommendedJobs(QuizResult q)
		{
			var jobs = new List<string>();

			// Dopasowania na podstawie typu pracy
			if (q.WorkType == "Umysłowo")
			{
				if (q.Skills.Contains("Programowanie") || q.TaskType == "Analityczne")
				{
					jobs.Add("Informatyk");
				}
				if (q.Skills.Contains("Zarządzanie") || q.Teamwork == "W zespole")
				{
					jobs.Add("Logistyk");
				}
				if (q.Skills.Contains("Komunikacja") || q.Values.Contains("Stabilność"))
				{
					jobs.Add("Ekonomista");
				}
				if (q.Skills.Contains("Kreatywność") || q.Values.Contains("Innowacyjność"))
				{
					jobs.Add("Biotechnolog");
				}
				if (q.Skills.Contains("Medycyna") || q.TaskType == "Analityczne")
				{
					jobs.Add("Lekarz");
				}
				if (q.Interests.Contains("Zdrowie") || q.Skills.Contains("Kreatywność"))
				{
					jobs.Add("Dietetyk");
					jobs.Add("Fizjoterapeuta");
				}
				if (q.Skills.Contains("Analiza") || q.Teamwork == "Samodzielnie")
				{
					jobs.Add("Audytor");
					jobs.Add("Prawnik");
				}
				if (q.Skills.Contains("Prawo") || q.TaskType == "Analityczne")
				{
					jobs.Add("Sędzia");
					jobs.Add("Adwokat");
				}
				if (q.Interests.Contains("Medycyna") || q.Skills.Contains("Precyzja"))
				{
					jobs.Add("Chirurg");
					jobs.Add("Ortodonta");
				}
				if (q.Skills.Contains("Zarządzanie") || q.Environment == "Zdalnie")
				{
					jobs.Add("Pilot");
				}
			}

			// Dopasowania na podstawie pracy fizycznej
			if (q.WorkType == "Fizycznie")
			{
				if (q.Environment == "W terenie" || q.Skills.Contains("Praktyczne"))
				{
					jobs.Add("Automatyk");
					jobs.Add("Elektryk");
					jobs.Add("Mechatronik");
				}
				if (q.WorkHours == "Zmienne" || q.TaskType == "Praktyczne")
				{
					jobs.Add("Spawacz");
					jobs.Add("Ślusarz");
				}
			}

			// Dopasowania na podstawie roli w zespole
			if (q.TeamRole == "Lider")
			{
				jobs.Add("Adwokat");
				jobs.Add("Prawnik");
			}

			if (q.TeamRole == "Analityk")
			{
				jobs.Add("Audytor");
			}

			if (q.TeamRole == "Kreator")
			{
				jobs.Add("Biotechnolog");
				jobs.Add("Informatyk");
			}

			// Zwracamy maksymalnie 3 najlepiej dopasowane zawody
			return jobs.Take(3).ToList();
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
