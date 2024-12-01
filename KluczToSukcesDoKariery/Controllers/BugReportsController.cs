using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KluczToSukcesDoKariery.Controllers
{
    [Authorize]
    public class BugReportsController : Controller
    {
        private readonly KluczToSukcesDoKarieryContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BugReportsController(KluczToSukcesDoKarieryContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var uid = _userManager.GetUserId(User);
            List<BugReport> reports;
            bool isAdmin = User.IsInRole("Administrator");

            if (isAdmin)
            {
                reports = _context.BugReports?.Include(br => br.User).ToList() ?? new List<BugReport>();
            } else
            {
                reports = _context.BugReports?.Where(r => r.UserId == uid).ToList() ?? new List<BugReport>();
            }

            ViewBag.Reports = reports;
            ViewBag.IsAdmin = isAdmin;
            return View();
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var report = _context.BugReports?.Include(br => br.User).FirstOrDefault(br => br.Id == id);
            var uid = _userManager.GetUserId(User);

            if (report == null || (!User.IsInRole("Administrator") && report.UserId != uid))
            {
                return RedirectToAction("Index");
            }

            var comments = _context.BugReportComments?.Where(brc => brc.BugReportId == report.Id).ToList();
            ViewBag.Comments = comments ?? new List<BugReportComment>();

            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var report = _context.BugReports?.FirstOrDefault(br => br.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            if (!Enum.TryParse(status, out BugReportStatus reportStatus))
            {
                return BadRequest();
            }

            report.Status = reportStatus;
            _context.Update(report);

            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([Bind("Content,BugReportId")] BugReportComment comment)
        {
            comment.DateCreated = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);
            _context.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { Id = comment.BugReportId });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description")] BugReport report)
        {
            report.DateCreated = DateTime.Now;
            report.Status = BugReportStatus.Open;
            report.UserId = _userManager.GetUserId(User);
            _context.Add(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { Id = report.Id });
        }
    }
}
