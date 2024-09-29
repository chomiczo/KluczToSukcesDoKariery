namespace KluczToSukcesDoKariery.Models
{
    public class QuizResult
    {
        public int Id { get; set; }

        // Dodaj właściwość UserId
        public string UserId { get; set; } // Id użytkownika

        public int CustomerModelId { get; set; } // Klucz obcy do CustomerModel
        public string WorkType { get; set; }
        public string Environment { get; set; }
        public string Teamwork { get; set; }
        public string Interests { get; set; }
        public string WorkHours { get; set; }
        public string Skills { get; set; }
        public string TaskType { get; set; }
        public string EmploymentType { get; set; }
        public string Values { get; set; }
        public string TeamRole { get; set; }

        // Relacja do CustomerModel
        public virtual CustomerModel User { get; set; }
    }
}
