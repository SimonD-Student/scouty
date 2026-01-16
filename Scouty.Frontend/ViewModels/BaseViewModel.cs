using CommunityToolkit.Mvvm.ComponentModel;

namespace Scouty.Frontend.ViewModels;

// "partial" est nécessaire car le Toolkit va générer du code en arrière-plan
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] // Génère automatiquement : public bool IsBusy { get; set; } + notification XAML
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    public bool IsNotBusy => !IsBusy;

    // Helper pour afficher une erreur
    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        IsBusy = false;
    }

    // Helper pour nettoyer l'état
    protected void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}