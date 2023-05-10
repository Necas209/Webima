using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models;

public class Cliente
{
    [Key] public string Id { get; set; }

    [Display(Name = "Data de nascimento")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime DataNascimento { get; set; }

    [ForeignKey(nameof(Id))] public Utilizador Utilizador { get; set; }

    public ICollection<CategoriaPreferida> CategoriasPreferidas { get; set; } = null!;

    public ICollection<Compra> Compras { get; set; } = null!;
}