using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models;

public class Compra
{
    public string ClienteId { get; init; }

    public int BilheteId { get; init; }

    [Display(Name = "Data Compra")] public DateTime DataCompra { get; init; }

    [Display(Name = "N.º Bilhetes")] public int NumeroBilhetes { get; init; }

    [ForeignKey(nameof(BilheteId))] public Bilhete Bilhete { get; init; }

    [ForeignKey(nameof(ClienteId))] public Cliente Cliente { get; set; }
}