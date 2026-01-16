using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scouty.Frontend.Services;
using Scouty.Shared.DTOs;
using Scouty.Shared.Enums;
using System.Collections.ObjectModel;


namespace Scouty.Frontend.ViewModels;

public partial class AddTransactionViewModel : BaseViewModel
{
    private readonly IBudgetService _budgetService;

    public AddTransactionViewModel(IBudgetService budgetService)
    {
        _budgetService = budgetService;

        // On remplit la liste des catégories à partir de l'Enum
        var categories = Enum.GetValues(typeof(TransactionCategory)).Cast<TransactionCategory>();
        Categories = new ObservableCollection<TransactionCategory>(categories);
        SelectedCategory = TransactionCategory.Autre; // Valeur par défaut
    }

    // --- PROPRIÉTÉS DU FORMULAIRE ---

    [ObservableProperty]
    private decimal? _amount; // Nullable pour que le champ soit vide au début

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private bool _isIncome; // False = Dépense, True = Entrée

    // Gestion du Picker (Liste déroulante)
    public ObservableCollection<TransactionCategory> Categories { get; }

    [ObservableProperty]
    private TransactionCategory _selectedCategory;


    // --- COMMANDES ---

    [RelayCommand]
    public void SetTypeToExpense() => IsIncome = false;

    [RelayCommand]
    public void SetTypeToIncome() => IsIncome = true;

    [RelayCommand]
    public async Task Save()
    {
        if (IsBusy) return;

        // 1. Validation basique
        if (Amount == null || Amount <= 0)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez entrer un montant valide.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(Description))
        {
            await Shell.Current.DisplayAlert("Erreur", "La description est obligatoire.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // 2. Création du DTO
            var dto = new CreateTransactionDto
            {
                Amount = Amount.Value,
                Description = Description,
                IsIncome = IsIncome,
                Category = SelectedCategory,
                Date = DateTime.UtcNow
            };

            // 3. Envoi au Backend
            bool success = await _budgetService.CreateTransactionAsync(dto);

            if (success)
            {
                await Shell.Current.DisplayAlert("Succès", "Transaction ajoutée !", "OK");
                // On retourne en arrière (ce qui rechargera la liste grâce au OnAppearing de BudgetPage)
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible d'enregistrer la transaction.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}