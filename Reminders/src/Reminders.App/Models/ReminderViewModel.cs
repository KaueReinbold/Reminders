using System;
using System.ComponentModel.DataAnnotations;

namespace Reminders.App.Models
{
    public class ReminderViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}."), StringLength(100), Display(Name = "Título")]
        public string Title { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}."), StringLength(500), Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}."), DataType(DataType.Date, ErrorMessage = "Digite uma data válida."), Display(Name = "Data Limite")]
        public DateTime? LimitDate { get; set; }

        [Display(Name = "Concluído")]
        public bool IsDone { get; set; }
    }
}
