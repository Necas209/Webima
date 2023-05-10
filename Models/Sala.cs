using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Webima.Models
{
    [Table("Sala")]
    public partial class Sala
    {
        public Sala()
        {
            Bilhetes = new HashSet<Bilhete>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} necessário.")]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "{0} necessária.")]
        [Display(Name = "Lotação")]
        [Range(0, 200, ErrorMessage = "A {0} tem de estar entre {1} e {2} lugares.")]
        public int Lotacao { get; set; }

        [Required]
        public bool Estado { get; set; } = true;

        [InverseProperty(nameof(Bilhete.IdSalaNavigation))]
        public virtual ICollection<Bilhete> Bilhetes { get; set; }
    }
}
