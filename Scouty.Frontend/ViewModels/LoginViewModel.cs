using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scouty.Frontend.Services;

namespace Scouty.Frontend.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    // Le Toolkit transforme cette méthode en "LoginCommand" pour le XAML
    [RelayCommand]
    async Task Login()
    {
        if (IsBusy) return;

        ClearError();

        // 1. Validation basique
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Veuillez remplir tous les champs.");
            return;
        }

        try
        {
            IsBusy = true;

            // 2. Appel au service (API plus tard)
            bool success = await _authService.LoginAsync(Email, Password);

            if (success)
            {
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            }
            else
            {
                SetError("Email ou mot de passe incorrect.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Erreur technique : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GoToRegister()
    {
        // Navigation vers la page de création
        // Attention : Il faudra définir la route "CreateAccountPage" dans AppShell
        await Shell.Current.GoToAsync("CreateAccountPage");
    }
}