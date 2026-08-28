using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class UsuarioCadView : ContentPage
{
	private readonly UsuarioCadViewModel _viewModel;
	public UsuarioCadView(UsuarioCadViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}
}