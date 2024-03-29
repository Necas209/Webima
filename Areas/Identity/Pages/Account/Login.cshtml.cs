﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Webima.Data;

namespace Webima.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LoginModel> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger,
        ApplicationDbContext context)
    {
        _signInManager = signInManager;
        _logger = logger;
        _context = context;
    }

    [BindProperty] public InputModel Input { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public string ReturnUrl { get; set; }

    [TempData] public string ErrorMessage { get; set; }

    public async Task OnGetAsync(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage)) ModelState.AddModelError(string.Empty, ErrorMessage);

        returnUrl ??= Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid) return Page();
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result =
            await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, false);
        if (result.Succeeded)
        {
            var id = (await _context.Utilizadores
                .SingleOrDefaultAsync(x => x.UserName == Input.UserName)).Id;
            HttpContext.Session.SetString("UserId", id);
            _logger.LogInformation("User logged in.");
            return LocalRedirect(returnUrl);
        }

        if (result.RequiresTwoFactor)
            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }

        ModelState.AddModelError(string.Empty, "Tentativa inválida de login.");
        return Page();

        // If we got this far, something failed, redisplay form
    }

    public class InputModel
    {
        [Required] public string UserName { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        [Display(Name = "Lembrar-me?")] public bool RememberMe { get; init; }
    }
}