using Microsoft.Extensions.Logging;
using CafeMissionario.ViewModels;
using CafeMissionario.Views;
using Microsoft.Maui.LifecycleEvents;

namespace CafeMissionario
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
                })
                .ConfigureLifecycleEvents(events =>
                {
#if WINDOWS
                    events.AddWindows(windowsLifecycleBuilder =>
                    {
                        windowsLifecycleBuilder.OnWindowCreated(window =>
                        {
                            // Pega o ID da janela nativa do Windows
                            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                            // Força a maximização
                            if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter p)
                            {
                                p.Maximize();
                            }
                        });
                    });
#endif
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // BUILDER VIEWMODELS
            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddTransient<PrincipalViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<UsuarioCadViewModel>();
            builder.Services.AddTransient<PedidoViewModel>();
            builder.Services.AddTransient<ProdutoCadViewModel>();
            builder.Services.AddTransient<ProdutoViewModel>();
            builder.Services.AddTransient<RelatoriosViewModel>();
            builder.Services.AddTransient<VendasDiaViewModel>();

            // BUILDER VIEWS
            builder.Services.AddTransient<SplashScreen>();
            builder.Services.AddTransient<PrincipalView>();
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<UsuarioCadView>();
            builder.Services.AddTransient<PedidoView>();
            builder.Services.AddTransient<ProdutoCadView>();
            builder.Services.AddTransient<ProdutoView>();
            builder.Services.AddTransient<RelatoriosView>();
            builder.Services.AddTransient<VendasDiaView>();

            return builder.Build();
        }
    }
}
