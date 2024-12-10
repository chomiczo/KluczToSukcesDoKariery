using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using KluczToSukcesDoKariery.Services;


namespace KluczToSukcesDoKariery.Controllers
{
    public class JobDescription
    {
        public List<string> WorkType { get; set; }
        public List<string> Environment { get; set; }
        public List<string> Teamwork { get; set; }
        public List<string> Interests { get; set; }
        public List<string> WorkHours { get; set; }
        public List<string> Skills { get; set; }
        public List<string> TaskType { get; set; }
        public List<string> EmploymentType { get; set; }
        public List<string> Values { get; set; }
        public List<string> TeamRole { get; set; }

        public int Score(QuizResult q)
        {
            int score = 0;

            if (WorkType.Contains(q.WorkType)) score++;
            if (Environment.Contains(q.Environment)) score++;
            if (Teamwork.Contains(q.Teamwork)) score++;
            score += Interests.Intersect(q.Interests).Count();
            if (WorkHours.Contains(q.WorkHours)) score++;
            score += Skills.Intersect(q.Skills).Count();
            if (TaskType.Contains(q.TaskType)) score++;
            if (EmploymentType.Contains(q.EmploymentType)) score++;
            score += Values.Intersect(q.Values).Count();
            if (TeamRole.Contains(q.TeamRole)) score++;

            return score;
        }
    }

    public class QuizyZawodowesController : Controller
    {
        public delegate void QuizSolvedHandler(object sender, QuizSolvedEventArgs args);
        public delegate void QuizJobSelectedHandler(object sender, QuizJobSelectedEventArgs args);
        public event QuizSolvedHandler QuizSolved;
        public event QuizJobSelectedHandler QuizJobSelected;

        private readonly KluczToSukcesDoKarieryContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly QuizService _quizService;

        private CustomerModel? GetCustomer()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var uid = _userManager.GetUserId(User);
                return _context.CustomerModel?.ToList().FirstOrDefault(c => c.UserId == uid, null);
            }
            return null;
        }

        public QuizyZawodowesController(KluczToSukcesDoKarieryContext context, UserManager<IdentityUser> userManager, QuizService quizService)
        {
            _context = context;
            _userManager = userManager;
            _quizService = quizService;

            QuizSolved += _quizService.OnQuizSolved;
            QuizJobSelected += _quizService.OnQuizJobSelected;
        }

        // GET: QuizyZawodowes/QuizSpecjalnosciowy
        public IActionResult QuizSpecjalnosciowy(string? selectedJob)
        {
            var customer = GetCustomer();
            if (selectedJob == null)
            {
                selectedJob = customer?.Job;
            } else { 
                QuizJobSelected?.Invoke(this, new QuizJobSelectedEventArgs { Customer = customer, SelectedJob = selectedJob });
            }

            var quiz = _context.QuizyZawodowe?.Include("Pytania.Odpowiedzi")?.FirstOrDefault(q => q.Tytul == selectedJob);

            if (quiz == null)
            {
                return RedirectToAction("QuizWstepny");
            }

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
                        Udzielona = p.Odpowiedzi?.First(o => o.Id == int.Parse(f.Value)),
                        Poprawna = p.Odpowiedzi?.First(o => o.Poprawna == true),
                        Odpowiedzi = p.Odpowiedzi?.ToList()
                    });
            quizResult.Wynik = userAnswers?.Where(x => x.Udzielona?.Poprawna ?? false).Sum(x => x.KoloUzyte ? x.Punktacja - 1 : x.Punktacja) ?? 0;
            quizResult.DataModyfikacji = DateTime.Now;


            if (customer != null && customer.UserId != null)
            {
                quizResult.Customer = customer;
                var task = _userManager.GetUserAsync(User);
                task.Wait();
                var user = task.Result;
                quizResult.User = user;
                _context.QuizyZawodoweWynik?.Update(quizResult);
                _context.SaveChanges();
                QuizSolved?.Invoke(this, new QuizSolvedEventArgs { Wynik = quizResult, User = user });
            }

            ViewBag.Answers = userAnswers;
            ViewBag.Total = quiz?.Pytania?.Sum(p => p.Punktacja);
            ViewBag.QuizResult = quizResult;
            return View();
        }

        public IActionResult Wynik()
        {
            var customer = GetCustomer();

            if (customer != null)
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
                    }
                    catch
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

        public IActionResult History()
        {
            var customer = GetCustomer();
            if (customer == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var results = _context.QuizyZawodoweWynik?.Include("Quiz").Where(qr => qr.Customer == customer).ToList();
            ViewBag.Results = results;

            return View();
        }

        public IActionResult Ranking()
        {
            var qr = _context.QuizyZawodoweWynik;
            var cm = _context.CustomerModel;
            var sums = qr.GroupBy(q => q.UserId).Select(
                group => new
                {
                    UserId = group.Key,
                    Sum = group.Sum(q => q.Wynik),
                    Count = group.Count()
                });
            var customerResults = sums.Join(cm, r => r.UserId, c => c.UserId, (r, c) => new
            {
                Customer = c,
                Wynik = r.Sum,
                r.Count,
                Streak = _context.QuizStreakForUser(c.User)
            }).ToList();
            customerResults.Sort((x, y) => (y.Wynik + y.Streak) - (x.Wynik + x.Streak));
            ViewBag.CustomerResults = customerResults.Take(10);

            return View();
        }

        private List<string> GetRecommendedJobs(QuizResult quizResult)
        {
            var jobScores = new List<(string, int)>();
            using (
                var fs = new FileStream(
                    Path.Combine(
                        Directory.GetCurrentDirectory(), "SeedData", "jobs.json"), FileMode.Open))
            {
                var doc = JsonDocument.Parse(fs);
                foreach (var jobObj in doc.RootElement.EnumerateObject())
                {
                    var jobName = jobObj.Name;
                    var jobDesc = jobObj.Value.Deserialize<JobDescription>();
                    jobScores.Add((jobName, jobDesc?.Score(quizResult) ?? 0));
                }
            }
            jobScores.Sort((x, y) => y.Item2 - x.Item2);

            return jobScores.Take(3).Select(x => x.Item1).ToList();
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
