using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ScamScraper.Models
{
    public class LoginUser
    {
        [Required]
        [Display(Name="Email:")]
        public string Email {get;set;}
        [Required]
        [Display(Name="Password:")]
        public string Password {get;set;}
    }
}