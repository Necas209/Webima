using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models
{
    [Table("Sessao")]
    public partial class Sessao
    {
        public Sessao()
        {
            Bilhetes = new HashSet<Bilhete>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} necessárias.")]
        public TimeSpan Horas { get; set; }

        [Required]
        public bool Estado { get; set; } = true;

        [InverseProperty(nameof(Bilhete.IdSessaoNavigation))]
        public virtual ICollection<Bilhete> Bilhetes { get; set; }
    }
}
