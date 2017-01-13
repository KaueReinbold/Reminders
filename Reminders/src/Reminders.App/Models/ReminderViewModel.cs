using System;
using System.ComponentModel.DataAnnotations;

namespace Reminders.App.Models
{
    public class ReminderViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Título")]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data Limite")]
        public DateTime LimitDate { get; set; }

        [Display(Name = "Concluído")]
        public bool IsDone { get; set; }
    }
}
