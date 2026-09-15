using CafeMissionario.Models;
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

        // Trava para evitar duplo clique
        private bool _isNavigating = false;

        [RelayCommand]
        private async Task AbrirTela(string nomeTela)
        {
            if (_isNavigating) return;

            try
            {
                _isNavigating = true;
                if (!string.IsNullOrWhiteSpace(nomeTela))
                {
                    await Shell.Current.GoToAsync(nomeTela);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível abrir a tela: {ex.Message}", "Ok");
            }
            finally
            {
                // Libera a trava após 300ms (tempo suficiente para a animação da tela)
                await Task.Delay(300);
                _isNavigating = false;
            }
        }

        [RelayCommand]
        private async Task AcessarComoVisitante()
        {
            try
            {
                SessaoSistema.UsuarioAtual = null;

                await Shell.Current.GoToAsync("//PrincipalView");
            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Erro", "Não foi possível abrir a tela principal", "Ok");
            }
        }

        [RelayCommand]
        private async Task Voltar()
        {
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Erro", $"Não foi possível voltar a tela: {e.Message}", "Ok");
            }
        }
    }
}
