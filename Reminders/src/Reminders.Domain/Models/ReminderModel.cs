using System;
using System.ComponentModel.DataAnnotations;

namespace Reminders.Domain.Models
{
    public class ReminderModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}."), StringLength(150), Display(Name = "Título")]
        public string title { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}."), StringLength(500), Display(Name = "Descrição")]
        public string description { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}.")]
        [DataType(DataType.Date, ErrorMessage = "Digite uma data válida.")]
        [Display(Name = "Data Limite")]
        public DateTime? limit_date { get; set; }

        [Display(Name = "Concluído")]
        public bool is_done { get; set; }
    }
}
