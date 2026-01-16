using Scouty.Frontend.ViewModels;

namespace Scouty.Frontend;

public partial class CalendarPage : ContentPage
{
    private readonly CalendarViewModel _viewModel;

    public CalendarPage(CalendarViewModel viewModel)
    {
        InitializeComponent();

        // C'est ici qu'on branche le "cerveau" à la page
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // On charge les données à chaque fois qu'on arrive sur la page
        await _viewModel.LoadEventsCommand.ExecuteAsync(null);
    }
}