using Microsoft.AspNetCore.Identity;

namespace KluczToSukcesDoKariery.Models
{
    public class QuizResult
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string WorkType { get; set; }
        public string Environment { get; set; }
        public string Teamwork { get; set; }
        public List<string> Interests { get; set; }
        public string WorkHours { get; set; }
        public List<string> Skills { get; set; }
        public string TaskType { get; set; }
        public string EmploymentType { get; set; }
        public List<string> Values { get; set; }
        public string TeamRole { get; set; }


        public QuizResult() { }


        public QuizResult(IFormCollection form) {
            foreach (var key in form.Keys)
            {
                var property = this.GetType().GetProperty(key);
                if (property == null) { continue; }

                if (property.PropertyType == typeof(List<string>))
                {
                    property.SetValue(this, form[key].ToList());
                } else
                {
                    property.SetValue(this, form[key].ToString());
                } 
            }
        }

        public QuizResult(IFormCollection form, IdentityUser user) : this(form)
        {
            this.UserId = user.Id;
        }
    }
}
