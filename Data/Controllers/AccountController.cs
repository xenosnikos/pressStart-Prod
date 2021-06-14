using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PressStart.Data.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        // private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager;
        // private readonly UserManager<IdentityUser> userManager;

        // public AccountController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) 
        // {
        //     this.roleManager = roleManager;
        //     this.userManager = userManager;
        // }

        // public async Task<IActionResult> DeleteUser(string id)
        // {
        //     var user = await userManager.FindByIdAsync(id);
        //     if(user == null) 
        //     {
        //         // ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
        //         // return View("NotFound");
        //         return NotFound();
        //     } else 
        //     {
        //         var result = await userManager.DeleteAsync(user);

        //         if (result.Succeeded)
        //         {
        //             return RedirectToAction("ListUsers");
        //         }
        //         foreach(var error in result.Errors)
        //         {
        //             ModelState.AddModelError("", error.Description);
        //         }
        //         return View("ListUsers");
        //     }
            
        // }

    }
}