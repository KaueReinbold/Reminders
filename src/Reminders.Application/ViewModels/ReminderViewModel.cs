using System;
using System.ComponentModel.DataAnnotations;

namespace Reminders.Application.ViewModels
{
    public class ReminderViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The field is Required")]
        [MaxLength(50)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The field is Required")]
        [MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field is Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid {0}")]
        [Display(Name = "Limit Date")]
        public DateTime LimitDate { get; set; }

        [Required(ErrorMessage = "The field is Required")]
        [Display(Name = "Is Done")]
        public bool IsDone { get; set; }
    }
}
