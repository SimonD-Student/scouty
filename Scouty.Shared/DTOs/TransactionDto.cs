using Scouty.Shared.Enums;

namespace Scouty.Shared.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public bool IsIncome { get; set; } // true = entrée (+), false = sortie (-)
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionCategory Category { get; set; }
    public string CreatorName { get; set; } = string.Empty;
}