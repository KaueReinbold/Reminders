using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Reminders.Domain.Models.Blockchain;

[FunctionOutput]
public class GetReminderOutput : IFunctionOutputDTO
{
    [Parameter("string", "text", 1)]
    public string Text { get; set; } = string.Empty;
    
    [Parameter("address", "owner", 2)]
    public string Owner { get; set; } = string.Empty;
}