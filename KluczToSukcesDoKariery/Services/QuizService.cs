using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace KluczToSukcesDoKariery.Services
{
    public class QuizSolvedEventArgs : EventArgs
    {
        public IdentityUser User { get; set; }
        public QuizyZawodoweWynik Wynik { get; set; }
    }

    public class QuizJobSelectedEventArgs : EventArgs
    {
        public CustomerModel Customer { get; set; }
        public string SelectedJob { get; set; }
    }

    public class QuizService
    {
        private readonly KluczToSukcesDoKarieryContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public QuizService(KluczToSukcesDoKarieryContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void OnQuizJobSelected(object sender, QuizJobSelectedEventArgs e)
        {
            var customer = e.Customer;
            customer.Job = e.SelectedJob;
            _context.CustomerModel?.Update(customer);
            _context.SaveChanges();
        }

        public void OnQuizSolved(object sender, QuizSolvedEventArgs e)
        {
            var quizResult = e.Wynik;
            var user = e.User;

            var badge = _context.QuizyZawodoweBadge?.FirstOrDefault(b => b.UserId == user.Id && b.QuizId == quizResult.QuizId);

            int level = 0;
            if (quizResult.Wynik >= 9) level = 1;
            if (quizResult.Wynik >= 27) level = 2;
            if (quizResult.Wynik >= 45) level = 3;
            if (quizResult.Wynik >= 63) level = 4;
            if (quizResult.Wynik >= 81) level = 5;

            if (badge == null)
            {
                badge = new QuizyZawodoweBadge();
                badge.UserId = user.Id;
                badge.QuizId = quizResult.QuizId;
                badge.Level = level;
            } else
            {
                if (level > badge.Level)
                {
                    badge.Level = level;
                }
            }
            _context.QuizyZawodoweBadge?.Update(badge);
            _context.SaveChanges();
        }
    }
}
