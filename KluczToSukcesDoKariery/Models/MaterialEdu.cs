namespace KluczToSukcesDoKariery.Models
{
    public class MaterialEdu
    {
        public int Id { get; set; }
        public string Tytul { get; set; }
        public string Opis { get; set; }
        public string Link { get; set; }
    }

    public class MaterialEduLike
    {
        public int Id { get; set; }
        public int MaterialEduId { get; set; }
        public string UserId { get; set; }

        public MaterialEduLike() { }

        public MaterialEduLike(int materialId, string userId)
        {
            this.MaterialEduId = materialId;
            this.UserId = userId;

        }
    }
}