using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PressStart.Data;
using PressStart.Models;
using Microsoft.AspNetCore.Identity;

namespace PressStart.Pages
{
    [Authorize]
    public class PlayGamesModel : PageModel
    {
        private readonly PressStartContext db;
        
        public PlayGamesModel(PressStartContext db)
        {
            this.db = db;
        } 

        [BindProperty(SupportsGet = true)]
        public int Id{get; set;}

        // public async Task OnGetAsync()
        // {
        //     Game = await db.Games.FindAsync(Id);

        //     UserList = await db.Users.ToListAsync();
        // }     

        [BindProperty]
        public Game Game {get;set;}

        // [BindProperty]
        // public Comment Comment {get;set;}
        
        [BindProperty]
        public IdentityUser ThisUser {get; set; }

        
        [BindProperty]
        public string CommentText {get; set; }

        public List<Comment> CommentList {get;set;} = new List<Comment>();
        public List<Microsoft.AspNetCore.Identity.IdentityUser> UserList { get; set; } = new List<Microsoft.AspNetCore.Identity.IdentityUser>(); 


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Game = await db.Games.FirstOrDefaultAsync(m => m.GameId == id);
            // Comment = await db.Comments.FirstOrDefaultAsync(m => m.CommentId == id);
            CommentList = await db.Comments.Where(x => x.Game.GameId == Game.GameId).ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || ThisUser == null || CommentText == null) {
                return Page();
            }
            
            var game = await db.Games.FirstOrDefaultAsync(x => x.GameId == id);
            var Comment = new PressStart.Models.Comment { CommentText = CommentText, User = ThisUser, Game = game };
            db.Comments.Add(Comment);
            await db.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
