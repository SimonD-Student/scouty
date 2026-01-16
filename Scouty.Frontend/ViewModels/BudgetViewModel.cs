using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scouty.Frontend.Services;
using Scouty.Shared.DTOs;
using SkiaSharp;
using Microcharts;
using System.Collections.ObjectModel;

namespace Scouty.Frontend.ViewModels;

// 1. On définit la classe LegendItem ici pour qu'elle soit visible
public class LegendItem
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Color Color { get; set; } // Couleur pour le XAML (BoxView)
}

public partial class BudgetViewModel : BaseViewModel
{
    private readonly IBudgetService _budgetService;
    private List<TransactionDto> _allTransactions = new();

    public BudgetViewModel(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private decimal _currentBalance;
    [ObservableProperty] private Chart _chart;

    public ObservableCollection<TransactionDto> Transactions { get; } = new();

    // Liste des éléments de la légende
    public ObservableCollection<LegendItem> LegendItems { get; } = new();

    [ObservableProperty] private bool _showExpenses = true; // True = Dépenses, False = Revenus

    // Couleurs fixes pour le graphique
    private readonly SKColor[] _colors = {
        SKColor.Parse("#04BBFF"), SKColor.Parse("#003E5C"), SKColor.Parse("#FFB900"),
        SKColor.Parse("#EF4444"), SKColor.Parse("#22C55E"), SKColor.Parse("#8B5CF6")
    };

    [RelayCommand]
    public async Task LoadTransactions()
    {
        if (IsBusy) return;
        IsBusy = true;
        IsLoading = true;

        try
        {
            _allTransactions = await _budgetService.GetTransactionsAsync();

            Transactions.Clear();
            foreach (var item in _allTransactions) Transactions.Add(item);

            var income = _allTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            var expense = _allTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);
            CurrentBalance = income - expense;

            UpdateChartAndLegend();
        }
        catch
        {
            // Gestion erreur silencieuse ou log
        }
        finally
        {
            IsLoading = false;
            IsBusy = false;
        }
    }

    // Boutons Dépenses / Revenus
    [RelayCommand]
    public void SetModeToExpenses()
    {
        ShowExpenses = true;
        UpdateChartAndLegend();
    }

    [RelayCommand]
    public void SetModeToIncome()
    {
        ShowExpenses = false;
        UpdateChartAndLegend();
    }

    private void UpdateChartAndLegend()
    {
        // Filtrer
        var targetTransactions = ShowExpenses
            ? _allTransactions.Where(t => !t.IsIncome).ToList()
            : _allTransactions.Where(t => t.IsIncome).ToList();

        // Grouper
        var groupedData = targetTransactions
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        LegendItems.Clear();
        var chartEntries = new List<ChartEntry>();
        int colorIndex = 0;

        foreach (var group in groupedData)
        {
            var skColor = _colors[colorIndex % _colors.Length];
            var mauiColor = Color.FromRgb(skColor.Red, skColor.Green, skColor.Blue);

            // Ajout Légende
            LegendItems.Add(new LegendItem
            {
                Name = group.Category.ToString(),
                Amount = group.Total,
                Color = mauiColor
            });

            // Ajout Graphique (Sans texte)
            chartEntries.Add(new ChartEntry((float)group.Total)
            {
                Color = skColor,
                ValueLabel = "",
                Label = ""
            });

            colorIndex++;
        }

        Chart = new DonutChart
        {
            Entries = chartEntries,
            HoleRadius = 0.65f,
            BackgroundColor = SKColors.Transparent,
            LabelTextSize = 0
        };
    }

    [RelayCommand]
    public async Task AddTransaction() => await Shell.Current.GoToAsync(nameof(AddTransactionPage));

    [RelayCommand]
    public async Task ResetHistory()
    {
        bool confirm = await Shell.Current.DisplayAlert("Reset ?",
            "Supprimer l'historique et faire un report de solde ?", "Oui", "Non");

        if (confirm)
        {
            if (await _budgetService.ResetBudgetAsync())
                await LoadTransactions();
        }
    }
}