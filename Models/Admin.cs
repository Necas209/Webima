using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models;

public sealed class Admin
{
    [Key] public string Id { get; set; }

    public string CriadorId { get; set; }

    [ForeignKey(nameof(CriadorId))] public Admin Criador { get; set; }

    [ForeignKey(nameof(Id))] public Utilizador Utilizador { get; set; }
}