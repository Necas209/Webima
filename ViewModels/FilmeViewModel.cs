using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webima.Models
{
    public class FilmeViewModel : Filme
    {
        [Required(ErrorMessage = "Data final necessária.")]
        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; } = DateTime.Now.Date;

        [Display(Name = "Sala")]
        public int IdSala { get; set; }

        [Display(Name = "Sessões")]
        public List<SessaoViewModel> Sessoes { get; set; }

        [Required(ErrorMessage = "Preço necessário.")]
        [Range(0, 30)]
        public decimal Preco { get; set; }
    }
}
