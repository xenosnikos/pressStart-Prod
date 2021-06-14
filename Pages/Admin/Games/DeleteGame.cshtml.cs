using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PressStart.Data;
using PressStart.Models;
// added
using Microsoft.AspNetCore.Authorization;

namespace PressStart.Pages
{
    [Authorize(Roles = "Admin")]
    public class DeleteGameModel : PageModel
    {
        private readonly PressStart.Data.PressStartContext _context;

        public DeleteGameModel(PressStart.Data.PressStartContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Game DelGame { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DelGame = await _context.Games.FirstOrDefaultAsync(m => m.GameId == id);

            if (DelGame == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DelGame = await _context.Games.FindAsync(id);

            if (DelGame != null)
            {
                _context.Games.Remove(DelGame);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage($"/Admin/Index");
        }
    }
}
