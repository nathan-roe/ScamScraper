using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ScamScraper.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        [Required(ErrorMessage="Field is required")]
        public string MessageText {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public List<Comment> Comments {get;set;}
        // public List<Answer> Answers {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}