using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models
{
    [Index(nameof(Nome), Name = "UQ__Categori__7D8FE3B22BBF0C19", IsUnique = true)]
    public partial class Categoria
    {
        public Categoria()
        {
            CliCats = new HashSet<CliCat>();
            Filmes = new HashSet<Filme>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} necessário.")]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        public bool Estado { get; set; } = true;

        [InverseProperty(nameof(CliCat.IdCatNavigation))]
        public virtual ICollection<CliCat> CliCats { get; set; }

        [InverseProperty(nameof(Filme.IdCatNavigation))]
        public virtual ICollection<Filme> Filmes { get; set; }
    }
}
