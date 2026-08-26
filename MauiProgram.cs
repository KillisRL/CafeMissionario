using Microsoft.Extensions.Logging;
using CafeMissionario.ViewModels;
using CafeMissionario.Views;

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

            // BUILDER VIEWS
            builder.Services.AddTransient<SplashScreen>();
            builder.Services.AddTransient<PrincipalView>();
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<UsuarioCadView>();
            builder.Services.AddTransient<PedidoView>();
            builder.Services.AddTransient<ProdutoCadView>();
            builder.Services.AddTransient<ProdutoView>();
            builder.Services.AddTransient<RelatoriosView>();

            return builder.Build();
        }
    }
}
