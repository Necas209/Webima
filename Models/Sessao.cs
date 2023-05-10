using System;
using System.ComponentModel.DataAnnotations;

namespace Webima.Models;

public class Sessao
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage = "{0} necessárias.")]
    public TimeSpan Horas { get; set; }

    [Required] public bool Estado { get; set; } = true;
}