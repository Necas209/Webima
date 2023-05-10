using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models;

public class CategoriaPreferida
{
    public string ClienteId { get; init; }

    public int CategoriaId { get; init; }

    [ForeignKey(nameof(CategoriaId))] public Categoria Categoria { get; set; }

    [ForeignKey(nameof(ClienteId))] public Cliente Cliente { get; set; }
}