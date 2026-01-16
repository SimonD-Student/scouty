using Scouty.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Scouty.Backend.Entities;

public class TransactionEntity
{
    [Key]
    public int Id { get; set; }

    public decimal Amount { get; set; }
    public bool IsIncome { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TransactionCategory Category { get; set; }

    // RELATIONS
    // Une transaction appartient à une Section (ex: Les transactions des Louveteaux ne sont pas celles des Pionniers)
    public Section Section { get; set; }

    // Qui a créé la transaction ?
    public int CreatedByUserId { get; set; }
    public UserEntity CreatedByUser { get; set; } // Navigation property
}