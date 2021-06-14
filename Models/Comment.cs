using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PressStart.Models
{
    public class Comment
    {
        public int CommentId {get; set;}

        [Required]
        public string CommentText {get; set;}

        public int Rating {get; set; }

        [Required]
        public IdentityUser User {get; set;}

        [Required]
        public Game Game {get; set;}
    }
}