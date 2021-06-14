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
    public class LoginModel : PageModel
    {
        private readonly PressStartContext db;
        private readonly ILogger<LoginModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public LoginModel(PressStartContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [Display(Name = "UserName")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }


        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, false);
                if (result.Succeeded)
                {
                    logger.LogInformation($"User {Input.UserName} logged in successfully.");
                    return RedirectToPage("/Index");
                }
                ModelState.AddModelError("", "Invalid Login Attempt");

            }
            return Page();
        }


    }
}
