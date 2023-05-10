using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Webima.Data;
using Webima.Models;

namespace Webima.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<RegisterModel> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger, IEmailSender emailSender, ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _applicationDbContext = applicationDbContext;
    }

    [BindProperty] public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (!ModelState.IsValid) return Page();
        var user = new Utilizador
        {
            UserName = Input.UserName,
            Email = Input.Email,
            Nome = Input.Nome
        };
        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

            // If we got this far, something failed, redisplay form
            return Page();
        }

        _logger.LogInformation("User created a new account with password.");
        await _userManager.AddToRoleAsync(user, "Cliente");

        var cliente = new Cliente { Id = user.Id, DataNascimento = Input.DataNascimento };
        _applicationDbContext.Clientes.Add(cliente);
        await _applicationDbContext.SaveChangesAsync();

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            null,
            new { area = "Identity", userId = user.Id, code, returnUrl },
            Request.Scheme) ?? string.Empty;

        await _emailSender.SendEmailAsync(Input.Email, "Confirme o seu email",
            $"Por favor, confirme a sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

        if (_userManager.Options.SignIn.RequireConfirmedAccount)
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });

        await _signInManager.SignInAsync(user, false);
        return LocalRedirect(returnUrl);
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; init; }

        [Required] [Display(Name = "Nome")] public string Nome { get; init; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de nascimento")]
        public DateTime DataNascimento { get; init; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; init; }

        [Required]
        [StringLength(100, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; init; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        public string ConfirmPassword { get; init; }
    }
}