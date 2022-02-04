using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Funcionario")]
    public partial class Funcionario
    {
        public Funcionario()
        {
            Filmes = new HashSet<Filme>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(9)]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }

        [Column("Id_Admin")]
        public int IdAdmin { get; set; }

        [ForeignKey(nameof(IdAdmin))]
        [InverseProperty(nameof(Admin.Funcionarios))]
        public virtual Admin IdAdminNavigation { get; set; }

        [ForeignKey(nameof(Id))]
        [InverseProperty(nameof(Utilizador.Funcionario))]
        public virtual Utilizador IdNavigation { get; set; }

        [InverseProperty(nameof(Filme.IdFuncNavigation))]
        public virtual ICollection<Filme> Filmes { get; set; }
    }
}
