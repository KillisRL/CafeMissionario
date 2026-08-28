using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CafeMissionario.ViewModels
{
    public partial class UsuarioCadViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private int _id;
        [ObservableProperty] private string _nome = string.Empty;
        [ObservableProperty] private string _senha = string.Empty;
        [ObservableProperty] private UsuarioTipo _tipoUsuario = UsuarioTipo.Colaborador;


        // Comandos
        [RelayCommand]
        public async Task SalvarCadastro()
        {
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlertAsync("Atenção", "Campos nome ou senha vazios", "Ok");
                return;
            }

            using var db = new AppDbContext();

            var usuarioBanco = db.Usuarios
                .FirstOrDefault(u => u.Nome.ToLower() == Nome.ToLower().Trim());

            if (usuarioBanco != null)
            {
                await Shell.Current.DisplayAlertAsync("Atenção", $"Usuário já cadastrado: {usuarioBanco.Nome}", "Ok");
                return;
            }

            var usuario = new Usuario()
            {
                Nome = this.Nome,
                Senha = this.Senha,
                Tipo = UsuarioTipo.Colaborador
            };

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            await Shell.Current.DisplayAlertAsync("Informação", "Usuário Cadastrado Com Sucesso!", "Ok");
            await Shell.Current.GoToAsync("LoginView");

            Nome = string.Empty;
            Senha = string.Empty;
            TipoUsuario = UsuarioTipo.Colaborador;

        }

        // Construtor
        public UsuarioCadViewModel()
        {

        }
    }
}
