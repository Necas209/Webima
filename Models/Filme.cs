using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webima.Models;

public class Filme
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage = "{0} necessário.")]
    [Display(Name = "Título")]
    [StringLength(50)]
    public string Titulo { get; set; }

    [Required] [StringLength(50)] public string Poster { get; set; }

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

    [Display(Name = "Categoria")] public int CategoriaId { get; set; }

    public string FuncionarioId { get; set; }

    [ForeignKey(nameof(CategoriaId))] public Categoria Categoria { get; set; }

    [ForeignKey(nameof(FuncionarioId))] public Funcionario IdFuncNavigation { get; set; }

    public ICollection<Bilhete> Bilhetes { get; set; }
}