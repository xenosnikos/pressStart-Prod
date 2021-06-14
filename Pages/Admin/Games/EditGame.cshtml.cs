using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using PressStart.Data;
using PressStart.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using PressStart.Data.Controllers;
using Amazon.S3.Model;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;

namespace PressStart.Pages.Admin.Games
{
    [Authorize(Roles = "Admin")]
    public class EditGameModel : PageModel
    {
        //public List<Game> GameList {get; set; } = new List<Game>();
        private PressStartContext db;
        private readonly ILogger<RegisterModel> logger;
        


        private IHostingEnvironment _environment;

        public EditGameModel(PressStartContext db, ILogger<RegisterModel> logger, IHostingEnvironment environment)
        {
            this.db = db;
            this.logger = logger;
            _environment = environment;
        }

        [BindProperty]
        public Game GameEdit {get; set; }

        [BindProperty]
        public int GameId {get; set;}

        [BindProperty, Required, MinLength(1), MaxLength(500), Display(Name = "GameName")]
        public string GameName { get; set; }

        [BindProperty, Required, MinLength(1), MaxLength(150), Display(Name = "GameType")]
        public string GameType { get; set; }

        [BindProperty, Required, MinLength(1), MaxLength(2000), Display(Name = "GamePath")]
        public string GamePath { get; set; }

        [BindProperty]
        public string ThumbnailPath { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]

        public IFormFile UploadThumb { get; set; }


        [BindProperty]

        public IFormFile UploadRom { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameEdit = await db.Games.FirstOrDefaultAsync(m => m.GameId == id);

            if (GameEdit == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            db.Attach(GameEdit).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(GameEdit.GameId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage($"/Admin/Index");
        }

        private bool GameExists(int id)
        {
            return db.Games.Any(e => e.GameId == id);
        }

    }
}
