using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class RelatoriosView : ContentPage
{
	public RelatoriosView(RelatoriosViewModel viewModel)
	{
		InitializeComponent();
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