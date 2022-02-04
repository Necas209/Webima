using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Admin")]
    public partial class Admin
    {
        public Admin()
        {
            Funcionarios = new HashSet<Funcionario>();
            InverseIdCrtNavigation = new HashSet<Admin>();
        }

        [Key]
        public int Id { get; set; }

        [Column("Id_Crt")]
        public int IdCrt { get; set; }

        [ForeignKey(nameof(IdCrt))]
        [InverseProperty(nameof(InverseIdCrtNavigation))]
        public virtual Admin IdCrtNavigation { get; set; }

        [ForeignKey(nameof(Id))]
        [InverseProperty(nameof(Utilizador.Admin))]
        public virtual Utilizador IdNavigation { get; set; }

        [InverseProperty(nameof(Funcionario.IdAdminNavigation))]
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        [InverseProperty(nameof(IdCrtNavigation))]
        public virtual ICollection<Admin> InverseIdCrtNavigation { get; set; }
    }
}
