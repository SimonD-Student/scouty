using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scouty.Frontend.Services;
using Scouty.Shared.Enums; // <--- Indispensable pour voir l'Enum
using System.Collections.ObjectModel;

namespace Scouty.Frontend.ViewModels;

public partial class CreateAccountViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    public CreateAccountViewModel(IAuthService authService)
    {
        _authService = authService;

        // Remplissage dynamique de la liste à partir de l'Enum Shared
        // Cela permet d'avoir toujours les mêmes sections que le Backend
        var sections = Enum.GetValues(typeof(Section)).Cast<Section>();
        SectionList = new ObservableCollection<Section>(sections);

        // On sélectionne la première valeur par défaut (ex: Baladins) pour éviter un vide
        SelectedSection = SectionList.FirstOrDefault();
    }

    [ObservableProperty] private string _firstname;
    [ObservableProperty] private string _lastname;
    [ObservableProperty] private string _totem;
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _password;

    // Changement ici : On utilise le type fort 'Section'
    [ObservableProperty]
    private Section _selectedSection;

    // La liste contient maintenant des objets Section
    public ObservableCollection<Section> SectionList { get; }

    [ObservableProperty] private bool _isChef;

    [RelayCommand]
    async Task Register()
    {
        if (IsBusy) return;
        ClearError();

        // Validation des champs texte
        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Merci de remplir les champs obligatoires.");
            return;
        }

        try
        {
            IsBusy = true;

            // Appel API simulé
            // Note : Pour l'instant, on passe encore les anciens paramètres.
            // Quand on fera le backend, on passera un objet "RegisterRequestDto" complet.
            // Ordre correct : Prénom, Nom, EMAIL, PASSWORD, TOTEM, Section, Chef
            bool success = await _authService.RegisterAsync(Firstname, Lastname, Email, Password, Totem, SelectedSection, IsChef);

            if (success)
            {
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}"); ;
            }
            else
            {
                SetError("Impossible de créer le compte.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Erreur : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GoBack()
    {
        // ".." signifie "revenir en arrière" dans le Shell
        await Shell.Current.GoToAsync("..");
    }
}