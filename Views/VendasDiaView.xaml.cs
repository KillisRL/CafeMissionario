using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class VendasDiaView : ContentPage
{
	public VendasDiaView(VendasDiaViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
		base.OnAppearing();

        if (BindingContext is VendasDiaViewModel viewModel)
        {
            viewModel.CarregarVendasHoje();
        }
    }
}