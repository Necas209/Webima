using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models;

public class Funcionario
{
    [Key] public string Id { get; set; }

    public string AdminId { get; set; }

    [ForeignKey(nameof(AdminId))] public Admin Admin { get; set; }

    [ForeignKey(nameof(Id))] public Utilizador Utilizador { get; set; }

    public ICollection<Filme> Filmes { get; set; } = null!;
}