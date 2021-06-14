using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PressStart.Data;
using PressStart.Models;

namespace PressStart.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PressStart.Data.PressStartContext db;
        public IndexModel(PressStartContext db) => this.db = db;
        public List<Game> GamesList { get; set; } = new List<Game>();
        public Game FeaturedGame { get; set; }  
        public Game PlayGame {get; set; }

        public async Task OnGetAsync()
        {
            GamesList = await db.Games.ToListAsync();
            FeaturedGame = GamesList.ElementAt(new Random().Next(GamesList.Count));
        }
    }
}
