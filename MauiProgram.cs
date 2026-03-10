using Microsoft.Extensions.Logging;
using MansuetoKarms.Services;
using MansuetoKarms.ViewModels;
using MansuetoKarms.Views;

namespace MansuetoKarms
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register HttpClient and Services
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<VehicleService>();

            // Register ViewModels
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<CreateViewModel>();
            builder.Services.AddTransient<UpdateViewModel>();

            // Register Views
            builder.Services.AddTransient<MainView>();
            builder.Services.AddTransient<CreateView>();
            builder.Services.AddTransient<UpdateView>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

