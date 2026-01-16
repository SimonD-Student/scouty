using Scouty.Shared.DTOs;

namespace Scouty.Frontend.Services;

public interface IBudgetService
{
    Task<BudgetSummaryDto?> GetSummaryAsync();
    Task<List<TransactionDto>> GetTransactionsAsync();
    Task<bool> CreateTransactionAsync(CreateTransactionDto transaction);

    Task<bool> ResetBudgetAsync();
}