using Microsoft.AspNetCore.Authorization; // Important pour [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scouty.Backend.Data;
using Scouty.Backend.Entities;
using Scouty.Shared.DTOs;
using Scouty.Shared.Enums;
using System.Security.Claims;

namespace Scouty.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Il faut être connecté pour accéder à tout ce contrôleur
public class BudgetController : ControllerBase
{
    private readonly ScoutyDbContext _context;

    public BudgetController(ScoutyDbContext context)
    {
        _context = context;
    }

    // GET: api/budget/summary
    // Renvoie le solde total pour la card du Dashboard
    [HttpGet("summary")]
    public async Task<ActionResult<BudgetSummaryDto>> GetSummary()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        // On récupère toutes les transactions de LA section de l'utilisateur
        var transactions = await _context.Transactions
            .Where(t => t.Section == user.Section)
            .ToListAsync();

        var income = transactions.Where(t => t.IsIncome).Sum(t => t.Amount);
        var expense = transactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

        return Ok(new BudgetSummaryDto
        {
            TotalIncome = income,
            TotalExpenses = expense,
            CurrentBalance = income - expense
        });
    }

    // GET: api/budget/transactions
    // Renvoie la liste pour la page Budget
    [HttpGet("transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactions()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        var list = await _context.Transactions
            .Where(t => t.Section == user.Section)
            .Include(t => t.CreatedByUser) // Pour avoir le nom du créateur
            .OrderByDescending(t => t.Date) // Les plus récentes en haut
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                IsIncome = t.IsIncome,
                Date = t.Date,
                Description = t.Description,
                Category = t.Category,
                CreatorName = t.CreatedByUser.Totem
            })
            .ToListAsync();

        return Ok(list);
    }

    // POST: api/budget
    // Ajoute une nouvelle dépense/entrée
    [HttpPost]
    public async Task<ActionResult> CreateTransaction(CreateTransactionDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        // Sécurité : Seuls les Chefs (ou trésoriers) peuvent ajouter ?
        // Pour l'instant on vérifie juste IsChef comme demandé
        if (!user.IsChef)
        {
            return Forbid("Seuls les chefs authorisés peuvent gérer le budget.");
        }

        var transaction = new TransactionEntity
        {
            Amount = dto.Amount,
            IsIncome = dto.IsIncome,
            Description = dto.Description,
            Category = dto.Category,
            Date = dto.Date,
            Section = user.Section, // On force la section de l'utilisateur
            CreatedByUserId = user.Id
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // Méthode utilitaire pour retrouver l'user grâce au Token
    private async Task<UserEntity?> GetCurrentUserAsync()
    {
        // Le Claim "NameIdentifier" contient l'ID (défini dans AuthController.CreateToken)
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdString, out int userId))
        {
            return await _context.Users.FindAsync(userId);
        }
        return null;
    }

    [HttpPost("reset")]
    public async Task<ActionResult> ResetBudget()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();
        if (!user.IsChef) return Forbid();

        // 1. Récupérer toutes les transactions de la section
        var transactions = await _context.Transactions
            .Where(t => t.Section == user.Section)
            .ToListAsync();

        if (!transactions.Any()) return Ok();

        // 2. Calculer le solde final avant suppression
        var income = transactions.Where(t => t.IsIncome).Sum(t => t.Amount);
        var expense = transactions.Where(t => !t.IsIncome).Sum(t => t.Amount);
        var balance = income - expense;

        // 3. Tout supprimer
        _context.Transactions.RemoveRange(transactions);

        // 4. Créer la transaction de "Report" si le solde n'est pas nul
        if (balance != 0)
        {
            var carryOver = new TransactionEntity
            {
                Amount = Math.Abs(balance), // Toujours positif
                IsIncome = balance > 0,     // Si positif = entrée, si négatif = dépense
                Description = "Report transactions précédentes",
                Category = TransactionCategory.Autre,
                Date = DateTime.UtcNow,
                Section = user.Section,
                CreatedByUserId = user.Id
            };
            _context.Transactions.Add(carryOver);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

}
