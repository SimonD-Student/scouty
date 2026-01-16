using Scouty.Frontend.ViewModels;

namespace Scouty.Frontend;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // On délègue tout le travail au ViewModel
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}