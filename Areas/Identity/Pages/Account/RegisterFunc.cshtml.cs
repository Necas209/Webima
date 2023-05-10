using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
public class FuncRegisterModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _sender;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public FuncRegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger, IEmailSender sender, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _sender = sender;
        _context = context;
    }

    [BindProperty] public InputModel Input { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string code)
    {
        if (userId == null || code == null) return RedirectToPage("/Index");
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound($"Unable to load user with ID '{userId}'.");
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "func", code);
        if (!result) return NotFound("Invalid registration token.");
        Input = new InputModel
        {
            AdminId = userId
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (await _userManager.FindByEmailAsync(Input.Email) != null)
            ModelState.AddModelError(string.Empty, "Email já em uso.");
        if (!ModelState.IsValid) return Page();
        var user = new Utilizador
        {
            UserName = Input.UserName,
            Email = Input.Email,
            Nome = Input.Nome,
            PhoneNumber = Input.Telefone
        };
        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

            // If we got this far, something failed, redisplay form
            return Page();
        }

        _logger.LogInformation("User created a new account with password.");
        await _userManager.AddToRoleAsync(user, "Funcionario");

        var funcionario = new Funcionario
        {
            Id = user.Id,
            AdminId = Input.AdminId
        };
        _context.Add(funcionario);
        await _context.SaveChangesAsync();

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            null,
            new { area = "Identity", userId = user.Id, code, returnUrl },
            Request.Scheme) ?? string.Empty;

        await _sender.SendEmailAsync(Input.Email, "Confirme o seu email",
            $"Por favor, confirme a sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

        if (_userManager.Options.SignIn.RequireConfirmedAccount)
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });

        await _signInManager.SignInAsync(user, false);
        return LocalRedirect(returnUrl);
    }

    public class InputModel
    {
        [Required] public string AdminId { get; init; }

        [Required] public string UserName { get; init; }

        [Required] public string Nome { get; init; }

        [Required]
        [StringLength(9)]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; init; }

        [Required] [EmailAddress] public string Email { get; init; }

        [Required]
        [StringLength(100, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        public string ConfirmPassword { get; init; }
    }
}