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
            return new Window(new AppShell());
        }
    }
}