using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class ProdutoView : ContentPage
{
    private readonly ProdutoViewModel _viewModel;
    public ProdutoView(ProdutoViewModel viewModel)
	{
		
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
        await _viewModel.ConsultarProdutosCommand.ExecuteAsync(null);
    }
}