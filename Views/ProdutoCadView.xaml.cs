using CafeMissionario.ViewModels;

namespace CafeMissionario.Views;

public partial class ProdutoCadView : ContentPage
{
	public ProdutoCadView(ProdutoCadViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}