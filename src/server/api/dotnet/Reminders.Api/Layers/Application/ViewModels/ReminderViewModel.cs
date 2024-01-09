namespace Reminders.Application.ViewModels;

public class ReminderViewModel
{
    [Key]
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "The field is Required")]
    [MaxLength(50, ErrorMessage = "The field {0} must be a text with a maximum length of '{1}'.")]
    [Display(Name = "Title", Prompt = "This is Reminder Title")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "The field is Required")]
    [MaxLength(200, ErrorMessage = "The field {0} must be a text with a maximum length of '{1}'.")]
    [Display(Name = "Description", Prompt = "This is Reminder Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "The field is Required")]
    [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
    [DataType(DataType.DateTime, ErrorMessage = "Invalid {0}")]
    [Display(Name = "Limit Date", Prompt = "This is Reminder Limit Date")]
    public DateTimeOffset LimitDate { get; set; }

    [Required(ErrorMessage = "The field is Required")]
    [Display(Name = "Is Done", Prompt = "This is Reminder Is Done flag")]
    public bool IsDone { get; set; }
}