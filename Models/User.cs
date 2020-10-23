using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ScamScraper.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }
        [NotMapped]
        [Compare("Password", ErrorMessage = "The passwords did not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password:")]
        public string Confirm { get; set; }
        public bool Admin {get;set;} = false;
        public List<Comment> Comments {get;set;}
        public List<Message> Messages {get;set;}
        public List<Association> Associations {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}