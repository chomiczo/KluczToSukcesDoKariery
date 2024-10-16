using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KluczToSukcesDoKariery.Models
{
    public class QuizyZawodowe
    {
        public int Id { get; set; }
        public string Tytul { get; set; } // Tytuł quizu
        public string Opis { get; set; } // Krótki opis quizu
        public DateTime? DataUtworzenia { get; set; } // Data utworzenia quizu
        public DateTime? DataModyfikacji { get; set; } // Data ostatniej modyfikacji quizu
        public List<Pytanie>? Pytania { get; set; } // Lista pytań w quizie

    }

    public class Pytanie
    {
        public int Id { get; set; }
        public int Punktacja { get; set; }
        public string Tekst { get; set; } // Treść pytania
        public List<Odpowiedz>? Odpowiedzi { get; set; } // Lista odpowiedzi na pytanie
    }

    public class Odpowiedz
    {
        public int Id { get; set; }
        public string Tekst { get; set; } // Treść odpowiedzi
        public bool Poprawna { get; set; } // Czy odpowiedź jest poprawna
    }

    public class QuizyZawodoweWynik
    {
        public int Id { get; set; }

        public int QuizId { get; set; }
        public virtual QuizyZawodowe? Quiz { get; set; }

        public string UserId { get; set; }
        public virtual IdentityUser? User {get;set;}

        public virtual CustomerModel? Customer { get; set; }

        public int Wynik { get; set; }

    }
}
