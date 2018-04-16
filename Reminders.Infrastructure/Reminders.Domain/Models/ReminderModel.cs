using System;
using System.ComponentModel.DataAnnotations;

namespace Reminders.Domain.Models
{
    public class ReminderModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime LimitDate { get; set; }

        [Required]
        public bool IsDone { get; set; }
    }
}
