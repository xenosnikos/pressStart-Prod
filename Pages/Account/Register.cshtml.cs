using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using PressStart.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace PressStart.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly PressStartContext db;
        private readonly ILogger<RegisterModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public RegisterModel(PressStartContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<RegisterModel> logger)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "UserName")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.UserName, Email = Input.Email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var result2 = await userManager.AddToRoleAsync(user, "User");
                    if (result2.Succeeded)
                    {
                        logger.LogInformation($"User {Input.UserName} create a new account with password");
                        return RedirectToPage("RegisterSuccess", new { email = Input.Email });
                    }
                    else
                    {
                        // FIXME: delete the user since role assignment failed, log the event, show error to the user
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
    }
}
