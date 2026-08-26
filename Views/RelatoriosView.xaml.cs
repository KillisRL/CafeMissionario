using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class RelatoriosView : ContentPage
{
    private readonly RelatoriosViewModel _viewModel;
	public RelatoriosView(RelatoriosViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RelatoriosViewModel viewModel)
        {
            viewModel.CarregarRelatório();
        }
    }
}