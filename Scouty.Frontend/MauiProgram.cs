using Microsoft.Extensions.Logging;
using Scouty.Frontend.Services;
using Scouty.Frontend.ViewModels;
using Microcharts.Maui;

namespace Scouty.Frontend
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    // Ajoute ici ta font Segoe MDL2 Assets si nécessaire, 
                    // sinon utilise une icône PNG/SVG pour être cross-platform
                });

            // 1. Enregistrer le Service
            // Singleton = une seule instance pour toute l'app
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IBudgetService, BudgetService>();
            builder.Services.AddSingleton<IEquipmentService, EquipmentService>();
            builder.Services.AddSingleton < ICalendarService,  CalendarService>();

            // 2. Enregistrer les ViewModels
            // Transient = nouvelle instance à chaque fois qu'on ouvre la page
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<CreateAccountViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<BudgetViewModel>();
            builder.Services.AddTransient<AddTransactionViewModel>();
            builder.Services.AddTransient<EquipmentViewModel>();
            builder.Services.AddTransient<CalendarViewModel>();
            builder.Services.AddTransient<AddEventViewModel>();

            // 3. Enregistrer les Pages
            builder.Services.AddTransient<MainPage>(); // Ta page de Login
            builder.Services.AddTransient<CreateAccountPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<BudgetPage>();
            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<EquipmentPage>();
            builder.Services.AddTransient<CalendarPage>();
            builder.Services.AddTransient<AddEventPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
