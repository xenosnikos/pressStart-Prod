using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Authorization;
using PressStart.Data;
using PressStart.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PressStart.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminIndexModel : PageModel
    {

        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        public List<Microsoft.AspNetCore.Identity.IdentityUser> UserList { get; set; } = new List<Microsoft.AspNetCore.Identity.IdentityUser>();
        public Microsoft.AspNetCore.Identity.IdentityUser ThisUser {get; set; }
        public List<Game> GameList {get; set; } = new List<Game>();

        private readonly PressStartContext db;
        private readonly ILogger<AdminIndexModel> _logger;
        public AdminIndexModel(PressStartContext db, ILogger<AdminIndexModel> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
                {
                    this.db = db;
                    _logger = logger;
                    this.roleManager = roleManager;
                    this.userManager = userManager;
                }

        public async Task OnGetAsync()
        {
            UserList = await db.Users.ToListAsync();
            GameList = await db.Games.ToListAsync();
        }

        

        public async Task<IActionResult> DeleteUser(string id)
        {
            ThisUser = await userManager.FindByIdAsync(id);
            if(ThisUser == null) 
            {
                // ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                // return View("NotFound");
                return NotFound();
            } else 
            {
                var result = await userManager.DeleteAsync(ThisUser);

                if (result.Succeeded)
                {
                    return RedirectToPage($"/Admin/Index");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return ViewComponent("ListUsers");
            }
            
        }
    }
}
