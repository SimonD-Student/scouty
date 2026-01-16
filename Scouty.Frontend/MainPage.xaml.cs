using Scouty.Frontend.ViewModels;

namespace Scouty.Frontend;

public partial class MainPage : ContentPage
{
    public MainPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // AJOUTE CECI :
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // On regarde si la clé existe déjà
        string token = await SecureStorage.Default.GetAsync("auth_token");

        // Si oui, on zappe le Login et on va direct au Dashboard
        if (!string.IsNullOrEmpty(token))
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }
    }
}