using Scouty.Frontend.ViewModels;

namespace Scouty.Frontend;

public partial class AddTransactionPage : ContentPage
{
    public AddTransactionPage(AddTransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}