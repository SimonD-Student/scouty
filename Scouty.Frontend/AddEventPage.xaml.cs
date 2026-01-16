using Scouty.Frontend.ViewModels;

namespace Scouty.Frontend;

public partial class AddEventPage : ContentPage
{
    private readonly AddEventViewModel _viewModel;

    public AddEventPage(AddEventViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUsersCommand.ExecuteAsync(null);
    }
}