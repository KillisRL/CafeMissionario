using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class PrincipalView : ContentPage
{
    private readonly PrincipalViewModel _viewModel;
	public PrincipalView(PrincipalViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PrincipalViewModel viewModel)
        {
            viewModel.CarregarUuario();
        }
    }
}