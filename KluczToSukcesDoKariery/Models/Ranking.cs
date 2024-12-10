namespace KluczToSukcesDoKariery.Models
{
    public class Ranking
    {
        public int Id { get; set; }
        public string? UserName { get; set; } // Nazwa użytkownika
        public int Points { get; set; } // Punkty w rankingu
        public DateTime? LastActive { get; set; } // Data ostatniej aktywności
        public bool IsActive { get; set; } // Status aktywności użytkownika
    }
}
