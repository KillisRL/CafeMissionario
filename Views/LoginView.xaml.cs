using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class LoginView : ContentPage
{
	private readonly LoginViewModel _viewModel;
	public LoginView(LoginViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}
}