using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KluczToSukcesDoKariery.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KluczToSukcesDoKariery.Data
{
    public class StringListConverter : ValueConverter<List<string>, string>
    {
        public StringListConverter() : base(
            v => string.Join('\x1e', v),
            v => new List<string>(v.Split(new [] {'\x1e'}))
        ) { }
    }

    public class KluczToSukcesDoKarieryContext : IdentityDbContext<IdentityUser>
    {
        public KluczToSukcesDoKarieryContext(DbContextOptions<KluczToSukcesDoKarieryContext> options)
            : base(options)
        {
        }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<List<string>>().HaveConversion<StringListConverter>();
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<QuizyZawodowe>().HasIndex(q => q.Tytul).IsUnique();
            builder.Entity<IdentityUser>().HasOne<CustomerModel>().WithOne(customer => customer.User);
            builder.Entity<QuizyZawodoweWynik>().HasIndex(wynik => new { wynik.QuizId, wynik.UserId }).IsUnique();
        }

        public DbSet<KluczToSukcesDoKariery.Models.MaterialyEdukacyjne>? MaterialyEdukacyjne { get; set; }

        public DbSet<KluczToSukcesDoKariery.Models.Ranking>? Ranking { get; set; }

        public DbSet<KluczToSukcesDoKariery.Models.QuizyZawodowe>? QuizyZawodowe { get; set; }

        public DbSet<KluczToSukcesDoKariery.Models.CustomerModel>? CustomerModel { get; set; }

        public DbSet<KluczToSukcesDoKariery.Models.QuizResult>? QuizResults { get; set; }  

        public DbSet<KluczToSukcesDoKariery.Models.QuizyZawodoweWynik>? QuizyZawodoweWynik { get; set; } 

        public DbSet<KluczToSukcesDoKariery.Models.Notatki>? Notatki { get; set; }
    }
}
