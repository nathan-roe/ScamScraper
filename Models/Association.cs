using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScamScraper.Models
{
    public class Association
    {
        [Key]
        public int AssociationId {get;set;}
        public int CommentId {get;set;}
        public int UserId {get;set;}
        public Comment Comment {get;set;}
        public User User {get;set;} 
    }
}