using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scouty.Frontend.Services;
using Scouty.Shared.DTOs;
using System.Collections.ObjectModel;

namespace Scouty.Frontend.ViewModels;

public partial class EquipmentViewModel : BaseViewModel
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentViewModel(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [ObservableProperty] private bool _isLoading;
    public ObservableCollection<EquipmentCategoryDto> Categories { get; } = new();

    [RelayCommand]
    public async Task LoadData()
    {
        if (IsBusy) return;
        IsBusy = true; IsLoading = true;
        try
        {
            var data = await _equipmentService.GetEquipmentAsync();
            Categories.Clear();
            foreach (var cat in data) Categories.Add(cat);
        }
        finally { IsBusy = false; IsLoading = false; }
    }

    [RelayCommand]
    public async Task AddCategory()
    {
        string name = await Shell.Current.DisplayPromptAsync("Nouvelle Catégorie", "Nom de la catégorie (ex: Cuisine) :");
        if (string.IsNullOrWhiteSpace(name)) return;

        if (await _equipmentService.CreateCategoryAsync(name))
            await LoadData();
    }

    [RelayCommand]
    public async Task DeleteCategory(EquipmentCategoryDto category)
    {
        bool confirm = await Shell.Current.DisplayAlert("Supprimer ?", $"Supprimer {category.Name} et tout son contenu ?", "Oui", "Non");
        if (confirm && await _equipmentService.DeleteCategoryAsync(category.Id))
            await LoadData();
    }

    [RelayCommand]
    public async Task AddItem(EquipmentCategoryDto category)
    {
        // On demande le nom
        string name = await Shell.Current.DisplayPromptAsync("Ajout Matériel", $"Ajouter un objet dans {category.Name} :");
        if (string.IsNullOrWhiteSpace(name)) return;

        // On demande la quantité
        string qtyString = await Shell.Current.DisplayPromptAsync("Quantité", "Combien ?", keyboard: Keyboard.Numeric);
        if (!int.TryParse(qtyString, out int qty)) qty = 1;

        var dto = new CreateEquipmentItemDto { Name = name, Quantity = qty, CategoryId = category.Id };
        if (await _equipmentService.AddItemAsync(dto))
            await LoadData();
    }

    [RelayCommand]
    public async Task DeleteItem(EquipmentItemDto item)
    {
        bool confirm = await Shell.Current.DisplayAlert("Retirer ?", $"Retirer {item.Name} ?", "Oui", "Non");
        if (confirm && await _equipmentService.DeleteItemAsync(item.Id))
            await LoadData();
    }
}