using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Calendar.Models;
using Scouty.Frontend.Services;
using Scouty.Shared.DTOs;
using System.Collections.ObjectModel;

namespace Scouty.Frontend.ViewModels;

public partial class CalendarViewModel : BaseViewModel
{
    private readonly ICalendarService _calendarService;
    private List<CalendarEventDto> _allEvents = new();

    public CalendarViewModel(ICalendarService calendarService)
    {
        _calendarService = calendarService;
        SelectedDate = DateTime.Now; // Par défaut aujourd'hui
    }

    // Collection spéciale pour le plugin (Date -> Liste d'objets)
    public EventCollection Events { get; } = new();

    // Liste filtrée affichée en dessous
    public ObservableCollection<CalendarEventDto> FilteredEvents { get; } = new();

    [ObservableProperty]
    private DateTime _selectedDate;

    // Quand la date change (clic sur le calendrier), on filtre
    async partial void OnSelectedDateChanged(DateTime value)
    {
        FilterEventsByDate(value);
    }

    [RelayCommand]
    public async Task LoadEvents()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            _allEvents = await _calendarService.GetMyEventsAsync();

            // 1. Remplir le Calendrier (pour les points de couleur)
            Events.Clear();
            foreach (var evt in _allEvents)
            {
                // Le plugin attend [Date, List<Objet>]
                if (!Events.ContainsKey(evt.StartDate))
                    Events[evt.StartDate] = new List<CalendarEventDto>();

                (Events[evt.StartDate] as List<CalendarEventDto>).Add(evt);
            }

            // Astuce pour forcer le rafraichissement du calendrier
            OnPropertyChanged(nameof(Events));

            // 2. Filtrer pour la date sélectionnée
            FilterEventsByDate(SelectedDate);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void FilterEventsByDate(DateTime date)
    {
        FilteredEvents.Clear();
        // On prend les events du jour
        var dayEvents = _allEvents.Where(e => e.StartDate.Date == date.Date).ToList();

        foreach (var e in dayEvents)
            FilteredEvents.Add(e);
    }

    [RelayCommand]
    public async Task AddEvent()
    {
        // Navigation vers page AddEvent
        await Shell.Current.GoToAsync(nameof(AddEventPage));
    }
}