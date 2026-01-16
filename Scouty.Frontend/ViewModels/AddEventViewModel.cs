using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scouty.Frontend.Services;
using Scouty.Shared.DTOs;
using Scouty.Shared.Enums;
using System.Collections.ObjectModel;

namespace Scouty.Frontend.ViewModels;

// Classe utilitaire pour cocher les sections
public class SectionSelection : ObservableObject
{
    public Section Value { get; set; }
    public string Name => Value.ToString();

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

// On doit wrapper le DTO User pour qu'il soit "Observable" (réactif aux clics)
public class UserSelection : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public partial class AddEventViewModel : BaseViewModel
{
    private readonly ICalendarService _calendarService;

    public AddEventViewModel(ICalendarService calendarService)
    {
        _calendarService = calendarService;

        // Initialiser les dates
        Date = DateTime.Now;
        Time = DateTime.Now.TimeOfDay;

        // Remplir la liste des sections disponibles
        var sections = Enum.GetValues(typeof(Section)).Cast<Section>();
        foreach (var sec in sections)
        {
            SectionsList.Add(new SectionSelection { Value = sec });
        }
    }

    // --- FORMULAIRE ---
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _description;
    [ObservableProperty] private DateTime _date;
    [ObservableProperty] private TimeSpan _time;

    // --- LISTES DE SÉLECTION ---
    public ObservableCollection<SectionSelection> SectionsList { get; } = new();
    public ObservableCollection<UserSelection> UsersList { get; } = new();

    // --- LOGIQUE ---

    [RelayCommand]
    public async Task LoadUsers()
    {
        if (UsersList.Count > 0) return; // Déjà chargé

        IsBusy = true;
        try
        {
            var users = await _calendarService.GetAllUsersAsync();
            UsersList.Clear();
            foreach (var u in users)
            {
                UsersList.Add(new UserSelection { Id = u.Id, Name = u.DisplayName, IsSelected = false });
            }
        }
        catch
        {
            await Shell.Current.DisplayAlert("Erreur", "Impossible de charger les scouts", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task Create()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Oups", "Il faut un titre !", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            // 1. Combiner Date et Heure
            var fullDate = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hours, Time.Minutes, 0);

            // 2. Récupérer les sélections
            var selectedSections = SectionsList.Where(s => s.IsSelected).Select(s => s.Value).ToList();
            var selectedUserIds = UsersList.Where(u => u.IsSelected).Select(u => u.Id).ToList();

            // 3. Créer le DTO
            var dto = new CreateEventDto
            {
                Title = Title,
                Description = Description,
                StartDate = fullDate,
                Sections = selectedSections,
                UserIds = selectedUserIds
            };

            // 4. Envoyer
            bool success = await _calendarService.CreateEventAsync(dto);
            if (success)
            {
                await Shell.Current.DisplayAlert("Succès", "Événement créé !", "OK");
                await Shell.Current.GoToAsync(".."); // Retour au calendrier
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Échec de la création", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Crash", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}