﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KluczToSukcesDoKariery.Models
{
public class CustomerModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Imię jest wymagane.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Pole Nazwisko jest wymagane.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public string Email { get; set; }

        public string? Job { get; set; }

        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }

        // Relacja do QuizResult
        public virtual ICollection<QuizResult>? QuizResults { get; set; }
        public virtual ICollection<QuizyZawodoweWynik> QuizyZawodoweWynik { get; set; }
    }
}
