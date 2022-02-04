using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Webima.Models
{
    [Table("Filme")]
    public partial class Filme
    {
        public Filme()
        {
            Bilhetes = new HashSet<Bilhete>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} necessário.")]
        [Display(Name = "Título")]
        [StringLength(50)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(50)]
        public string Poster { get; set; }

        [Required(ErrorMessage = "Data de estreia necessária.")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime Estreia { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "{0} necessária.")]
        public string Sinopse { get; set; }

        [Required(ErrorMessage = "{0} necessário.")]
        [StringLength(50)]
        public string Realizador { get; set; }

        [Required(ErrorMessage = "{0} necessário.")]
        [StringLength(100)]
        public string Elenco { get; set; }

        [Required(ErrorMessage = "{0} necessário.")]
        public int Ano { get; set; } = DateTime.Now.Year;

        [Required(ErrorMessage = "{0} necessária.")]
        [Range(0, 250, ErrorMessage = "{0} entre {1} e {2} minutos.")]
        [Display(Name = "Duração")]
        public int Duracao { get; set; }

        [Required(ErrorMessage = "{0} necessário.")]
        [DataType(DataType.Url)]
        public string Trailer { get; set; }

        [Column("Id_Cat")]
        [Display(Name = "Categoria")]
        public int IdCat { get; set; }

        [Column("Id_Func")]
        public int IdFunc { get; set; }

        [ForeignKey(nameof(IdCat))]
        [InverseProperty(nameof(Categoria.Filmes))]
        public virtual Categoria IdCatNavigation { get; set; }

        [ForeignKey(nameof(IdFunc))]
        [InverseProperty(nameof(Funcionario.Filmes))]
        public virtual Funcionario IdFuncNavigation { get; set; }

        [InverseProperty(nameof(Bilhete.IdFilmeNavigation))]
        public virtual ICollection<Bilhete> Bilhetes { get; set; }
    }
}
