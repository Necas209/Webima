using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Cli_Cat")]
    public partial class CliCat
    {
        [Column("Id_Cliente")]
        public int IdCliente { get; set; }

        [Column("Id_Cat")]
        public int IdCat { get; set; }

        [ForeignKey(nameof(IdCat))]
        [InverseProperty(nameof(Categoria.CliCats))]
        public virtual Categoria IdCatNavigation { get; set; }

        [ForeignKey(nameof(IdCliente))]
        [InverseProperty(nameof(Cliente.Categorias))]
        public virtual Cliente IdClienteNavigation { get; set; }
    }
}
