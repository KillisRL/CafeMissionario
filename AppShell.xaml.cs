namespace CafeMissionario
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // ROTAS DAS VIEWS
            Routing.RegisterRoute(nameof(Views.PrincipalView), typeof(Views.PrincipalView));
            Routing.RegisterRoute(nameof(Views.LoginView), typeof(Views.LoginView));
            Routing.RegisterRoute(nameof(Views.UsuarioCadView), typeof(Views.UsuarioCadView));
            Routing.RegisterRoute(nameof(Views.PedidoView), typeof(Views.PedidoView));
            Routing.RegisterRoute(nameof(Views.ProdutoCadView), typeof(Views.ProdutoCadView));
            Routing.RegisterRoute(nameof(Views.ProdutoView), typeof(Views.ProdutoView));
            Routing.RegisterRoute(nameof(Views.RelatoriosView), typeof(Views.RelatoriosView));
        }
    }
}
