using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scouty.Frontend.Services;
using Scouty.Shared.DTOs;

namespace Scouty.Frontend.ViewModels;

// IMPORTANT : "partial" est obligatoire pour que [ObservableProperty] fonctionne !
public partial class DashboardViewModel : BaseViewModel
{
    private readonly IBudgetService _budgetService;

    public DashboardViewModel(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    // Le Toolkit va générer automatiquement la propriété publique "IsLoading" (avec majuscule)
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private BudgetSummaryDto? _summary;

    [ObservableProperty]
    private string _welcomeMessage = "Chargement...";

    // Commande appelée à l'affichage de la page
    [RelayCommand]
    public async Task LoadData()
    {
        IsLoading = true; // Maintenant ça compile grâce à "partial"

        // 1. Récupérer le nom pour l'accueil
        string userName = await SecureStorage.Default.GetAsync("user_name") ?? "Scout";
        WelcomeMessage = $"Bienvenue, {userName} !";

        // 2. Récupérer le budget
        var summary = await _budgetService.GetSummaryAsync();
        if (summary != null)
        {
            Summary = summary;
        }

        IsLoading = false;
    }

    // Commande pour aller au détail (Restriction Chef)
    [RelayCommand]
    public async Task GoToBudgetDetails()
    {
        string isChefString = await SecureStorage.Default.GetAsync("is_chef");
        bool isChef = isChefString == "True";

        if (!isChef) return;

        // Navigation réelle vers la page Budget
        await Shell.Current.GoToAsync(nameof(BudgetPage));
    }

    [RelayCommand]
    public async Task GoToEquipment()
    {
        // Accessible par tout le monde, pas de restriction "IsChef" ici
        await Shell.Current.GoToAsync(nameof(EquipmentPage));
    }


    [RelayCommand]
    public async Task GoToCalendar()
    {
        await Shell.Current.GoToAsync(nameof(CalendarPage));
    }

    // Commande de Déconnexion
    [RelayCommand]
    public async Task Logout()
    {
        SecureStorage.Default.Remove("auth_token");
        SecureStorage.Default.Remove("user_name");
        SecureStorage.Default.Remove("is_chef");

        await Shell.Current.GoToAsync("//MainPage");
    }
}