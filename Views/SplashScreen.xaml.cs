using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class SplashScreen : ContentPage
{
	public SplashScreen(BaseViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}