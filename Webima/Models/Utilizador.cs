using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Utilizador")]
    public partial class Utilizador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [InverseProperty("IdNavigation")]
        public virtual Admin Admin { get; set; }

        [InverseProperty("IdNavigation")]
        public virtual Cliente Cliente { get; set; }

        [InverseProperty("IdNavigation")]
        public virtual Funcionario Funcionario { get; set; }
    }
}
