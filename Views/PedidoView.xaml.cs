using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class PedidoView : ContentPage
{
    private readonly PedidoViewModel _viewModel;
	public PedidoView(PedidoViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PedidoViewModel viewModel)
        {
            viewModel.CarregarCardapio();
        }
    }
}