using System.Text.RegularExpressions;
using KluczToSukcesDoKariery.Data;
using KluczToSukcesDoKariery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class SeedData
{
    private static readonly string SEED_DATA_PATH = Path.Combine(AppContext.BaseDirectory, "SeedData");
    private static readonly string QUIZ_DATA_PATH = Path.Combine(SEED_DATA_PATH, "Quizes");

    private static async Task LoadQuizFromFile(KluczToSukcesDoKarieryContext context, string path)
    {
        var src = await File.ReadAllTextAsync(path);
        var quizName = Path.GetFileNameWithoutExtension(path);

        var quiz = context.QuizyZawodowe?.Include("Pytania.Odpowiedzi").FirstOrDefault(q => q.Tytul == quizName);
        if (quiz == null)
        {
            quiz = new QuizyZawodowe();
            quiz.Tytul = quizName;
            quiz.DataUtworzenia = DateTime.Now;
            quiz.Opis = $"Ten quiz pozwoli ci sprawdzić, czy pasujesz na stanowisko {quizName}";
        }
        quiz.DataModyfikacji = DateTime.Now;
        quiz.Pytania = new List<Pytanie>();


        int pkt = 1;
        Pytanie pyt = new Pytanie();

        foreach (var line in File.ReadLines(path))
        {
            var match = Regex.Match(line, @"PYTANIA ZA (?<pkt>\d+)PKT:");
            if (match.Success)
            {
                pkt = int.Parse(match.Groups[groupname: "pkt"].Value);
                continue;
            }

            match = Regex.Match(line, @"(?<id>\d+)\.(?<text>.+)");
            if (match.Success)
            {
                var text = match.Groups["text"].Value;

                pyt = new Pytanie();

                pyt.Tekst = text;
                pyt.Punktacja = pkt;
                pyt.Odpowiedzi = new List<Odpowiedz>();

                quiz.Pytania.Add(pyt);

                continue;
            }

            match = Regex.Match(line, @"[A-D]\)\s*(?<text>.+)");
            if (match.Success)
            {
                var odp = new Odpowiedz();
                odp.Poprawna = false;
                var text = match.Groups["text"].Value;
                if (text.EndsWith("(✔️)"))
                {
                    text = text.Substring(0, text.Length - 5);
                    odp.Poprawna = true;
                }
                odp.Tekst = text;
                pyt.Odpowiedzi.Add(odp);

            }
        }

        var quizDb = context.QuizyZawodowe?.Update(quiz);

        await context.SaveChangesAsync();

        context.Database.ExecuteSqlRaw("DELETE o FROM Odpowiedz o JOIN Pytanie p ON o.PytanieId = p.Id WHERE p.QuizyZawodoweId IS NULL;");
        context.Database.ExecuteSqlRaw("DELETE FROM Pytanie WHERE QuizyZawodoweId IS NULL;");
        await context.SaveChangesAsync();
    }

    private static async Task<int> InsertQuizData(KluczToSukcesDoKarieryContext context)
    {
        foreach (var fname in Directory.EnumerateFiles(QUIZ_DATA_PATH, "*.txt"))
        {

            Console.WriteLine($"Reading data from {fname}");
            await LoadQuizFromFile(context, fname);

        }
        return await context.SaveChangesAsync();
    }

    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, KluczToSukcesDoKarieryContext context)
    {
        Console.WriteLine("Initialize method called.");
        string roleName = "Administrator";
        IdentityResult roleResult;

        // Sprawdź, czy rola już istnieje
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Jeśli rola nie istnieje, utwórz ją
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Sprawdź, czy administrator już istnieje
        IdentityUser admin = await userManager.FindByEmailAsync("admin@admin.com");

        if (admin == null)
        {
            // Utwórz administratora tylko jeśli nie istnieje
            admin = new IdentityUser()
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            };

            // Jeśli administrator nie istnieje, utwórz go
            IdentityResult result = await userManager.CreateAsync(admin, "Admin123$");

            if (result.Succeeded)
            {
                admin.EmailConfirmed = true;
                await userManager.UpdateAsync(admin);

                // Przypisz rolę "Administrator" do administratora
                await userManager.AddToRoleAsync(admin, roleName);
            }
        }

        var customer = context.CustomerModel?.FirstOrDefault() ?? new CustomerModel();
        customer.UserId = admin.Id;
        customer.FirstName = "Admin";
        customer.LastName = "Administracki";
        customer.Email = admin.Email;
        context.CustomerModel?.Update(customer);
        await context.SaveChangesAsync();

        await InsertQuizData(context);
    }
}
