using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class PrincipalView : ContentPage
{
	public PrincipalView(PrincipalViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}