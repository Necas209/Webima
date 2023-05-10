using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models;

public class Bilhete
{
    [Key] public int Id { get; set; }

    public int FilmeId { get; set; }

    public int SessaoId { get; set; }

    public int SalaId { get; set; }

    [Column(TypeName = "money")] public decimal Preco { get; set; }

    [Column(TypeName = "date")] public DateTime Data { get; set; }

    [ForeignKey(nameof(FilmeId))] public Filme Filme { get; set; }

    [ForeignKey(nameof(SalaId))] public Sala Sala { get; set; }

    [ForeignKey(nameof(SessaoId))] public Sessao Sessao { get; set; }
}