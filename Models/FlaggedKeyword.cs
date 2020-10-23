using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ScamScraper.Models
{
    public class FlaggedKeyword
    {
        [Key]
        public int FlaggedKeywordId {get;set;}
        public string FlaggedKeywordWord {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}