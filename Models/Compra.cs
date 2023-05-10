using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Compra")]
    public partial class Compra
    {
        [Column("Id_Cliente")]
        public int IdCliente { get; set; }

        [Column("Id_Bil")]
        public int IdBil { get; set; }

        [Display(Name = "Data Compra")]
        [Column("Data_Compra", TypeName = "datetime")]
        public DateTime DataCompra { get; set; }

        [Display(Name = "N.º Bilhetes")]
        [Column("Num_Bil")]
        public int NumBil { get; set; }

        [ForeignKey(nameof(IdBil))]
        [InverseProperty(nameof(Bilhete.Compras))]
        public virtual Bilhete IdBilNavigation { get; set; }

        [ForeignKey(nameof(IdCliente))]
        [InverseProperty(nameof(Cliente.Compras))]
        public virtual Cliente IdClienteNavigation { get; set; }
    }
}
