using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Webima.Models;

public class Utilizador : IdentityUser
{
    [Required] [StringLength(100)] public string Nome { get; set; }
}