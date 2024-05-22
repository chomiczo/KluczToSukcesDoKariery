namespace KluczToSukcesDoKariery.Models
{
    public class Onas
    {
        public int Id { get; set; }
        public string? Tytul { get; set; } // Tytuł inicjatywy lub projektu
        public string? Opis { get; set; } // Opis inicjatywy lub projektu
        public string? Cel { get; set; } // Cele projektu lub inicjatywy
        public string? Tworcy { get; set; } // Informacje o założycielach lub członkach projektu
    }
}
