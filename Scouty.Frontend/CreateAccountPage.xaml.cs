using Scouty.Frontend.ViewModels;

namespace Scouty.Frontend;

public partial class CreateAccountPage : ContentPage
{
    public CreateAccountPage(CreateAccountViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}