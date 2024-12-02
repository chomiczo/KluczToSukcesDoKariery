using Microsoft.AspNetCore.SignalR;

namespace KluczToSukcesDoKariery.Models
{
    public class Notatki
    {
        public int Id { get; set; }
        public string Tytul { get; set; }
        public string Tresc { get; set; }
        public DateTime? DataDodania { get; set; }
        public string UserId { get; set; }
    }
}