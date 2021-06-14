using System.ComponentModel.DataAnnotations;

namespace PressStart.Models
{
    public class Game
    {
        public int GameId {get; set;}

        [Required, MinLength(1), MaxLength(500)]
        public string GameName {get; set;}

        [Required, MinLength(1), MaxLength(150)]
        public string GameType {get; set;}

        [Required, MinLength(1), MaxLength(2000)]
        public string GamePath {get; set;}

        [Required]
        public string ThumbnailPath {get; set;}

        [Required]
        public string Description {get; set;}
    }
}