using Scouty.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Scouty.Shared.DTOs;

public class CreateTransactionDto
{
    [Required]
    public decimal Amount { get; set; }

    [Required]
    public bool IsIncome { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TransactionCategory Category { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
}