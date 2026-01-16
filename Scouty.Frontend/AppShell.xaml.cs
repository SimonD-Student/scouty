namespace Scouty.Frontend;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(CreateAccountPage), typeof(CreateAccountPage));
        Routing.RegisterRoute(nameof(BudgetPage), typeof(BudgetPage));
        Routing.RegisterRoute(nameof(AddTransactionPage), typeof(AddTransactionPage));
        Routing.RegisterRoute(nameof(EquipmentPage), typeof(EquipmentPage));
        Routing.RegisterRoute(nameof(CalendarPage), typeof(CalendarPage));
        Routing.RegisterRoute(nameof(AddEventPage), typeof(AddEventPage));

    }

}