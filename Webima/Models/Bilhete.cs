using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Bilhete")]
    public partial class Bilhete
    {
        public Bilhete()
        {
            Compras = new HashSet<Compra>();
        }

        [Key]
        public int Id { get; set; }

        [Column("Id_Filme")]
        public int IdFilme { get; set; }

        [Column("Id_Sessao")]
        public int IdSessao { get; set; }

        [Column("Id_Sala")]
        public int IdSala { get; set; }

        [Column(TypeName = "money")]
        public decimal Preco { get; set; }

        [Column(TypeName = "date")]
        public DateTime Data { get; set; }

        [ForeignKey(nameof(IdFilme))]
        [InverseProperty(nameof(Filme.Bilhetes))]
        public virtual Filme IdFilmeNavigation { get; set; }

        [ForeignKey(nameof(IdSala))]
        [InverseProperty(nameof(Sala.Bilhetes))]
        public virtual Sala IdSalaNavigation { get; set; }

        [ForeignKey(nameof(IdSessao))]
        [InverseProperty(nameof(Sessao.Bilhetes))]
        public virtual Sessao IdSessaoNavigation { get; set; }

        [InverseProperty(nameof(Compra.IdBilNavigation))]
        public virtual ICollection<Compra> Compras { get; set; }
    }
}
