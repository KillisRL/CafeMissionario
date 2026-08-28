using Microsoft.Extensions.DependencyInjection;

namespace CafeMissionario
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            CafeMissionario.Helpers.DbInitializer.Init();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Instancia a janela diretamente com a AppShell e define as dimensões
            var window = new Window(new AppShell())
            {
                Width = 1024,
                Height = 700,
                MinimumWidth = 850,
                MinimumHeight = 600
            };

            return window;
        }
    }
}