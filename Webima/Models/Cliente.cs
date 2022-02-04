using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Cliente")]
    public partial class Cliente
    {
        public Cliente()
        {
            Categorias = new HashSet<CliCat>();
            Compras = new HashSet<Compra>();
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Data de nascimento")]
        [Column("Data_Nasc", TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime DataNasc { get; set; }

        [ForeignKey(nameof(Id))]
        [InverseProperty(nameof(Utilizador.Cliente))]
        public virtual Utilizador IdNavigation { get; set; }

        [InverseProperty(nameof(CliCat.IdClienteNavigation))]
        public virtual ICollection<CliCat> Categorias { get; set; }

        [InverseProperty(nameof(Compra.IdClienteNavigation))]
        public virtual ICollection<Compra> Compras { get; set; }
    }
}
