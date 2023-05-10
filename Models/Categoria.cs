using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Webima.Models;

[Index(nameof(Nome), IsUnique = true)]
public class Categoria
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(50)] public string Nome { get; set; }

    [Required] public bool Estado { get; set; } = true;

    public ICollection<Filme> Filmes { get; set; } = null!;
}