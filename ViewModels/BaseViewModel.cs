using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CafeMissionario.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        // Propriedades
        [ObservableProperty] private bool _isAdm;

        [ObservableProperty] private string _nomeUsuario = string.Empty;

        [ObservableProperty] private string _title = string.Empty;

        // Construtor
        public BaseViewModel()
        {

        }

        // Comandos
        [RelayCommand]
        private async Task AbrirTela(string nomeTela)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(nomeTela))
                {
                    await Shell.Current.GoToAsync(nomeTela);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Erro", "Não foi possível abrir a tela", "Ok");
            }
            
        }
    }
}
