using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CafeMissionario.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private int _id;
        [ObservableProperty] private string _nome = string.Empty;
        [ObservableProperty] private string _senha = string.Empty;
        [ObservableProperty] private UsuarioTipo _tipoUsuario = UsuarioTipo.Colaborador;

        // Comandos
        [RelayCommand]
        public async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Nome) ||  string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlertAsync("Atenção", "Campo nome ou senha vazio", "Ok");
                return;
            }

            using var db = new AppDbContext();

            var usuarioBanco = db.Usuarios
                .FirstOrDefault(u => u.Nome.ToLower() == Nome.Trim().ToLower() &&
                u.Senha == Senha);

            if (usuarioBanco == null)
            {
                await Shell.Current.DisplayAlertAsync("Acesso Negado", "Usuário ou senha incorretos.", "Ok");
                return;
            }

            Senha = string.Empty;

            SessaoSistema.UsuarioAtual = usuarioBanco;

            await Shell.Current.DisplayAlertAsync("Boas-vindas", $"Seja bem-vindo(a), {usuarioBanco.Nome}!", "Ok");

            await Shell.Current.GoToAsync("PrincipalView");
        }

        // Construtor
        public LoginViewModel()
        {

        }
    }
}
