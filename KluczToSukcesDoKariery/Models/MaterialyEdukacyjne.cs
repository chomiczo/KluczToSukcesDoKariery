using System;

namespace KluczToSukcesDoKariery.Models
{
    public class MaterialyEdukacyjne
    {
        public int Id { get; set; }
        public string? Tytul { get; set; } // Tytuł materiału edukacyjnego
        public string? Opis { get; set; } // Opis materiału edukacyjnego
        public string? Autor { get; set; } // Autor materiału edukacyjnego
        public DateTime? DataDodania { get; set; } // Data dodania materiału edukacyjnego
        public string? Url { get; set; } // URL do materiału edukacyjnego
    }
}
