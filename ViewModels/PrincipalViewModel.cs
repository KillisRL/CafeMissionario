using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CafeMissionario.ViewModels
{
    public partial class PrincipalViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private string _nomeUsuario = "Usuário";
        [ObservableProperty] private string _cargoUsuario = "Colaborador";

        // Métodos
        public void CarregarUuario()
        {
            if (SessaoSistema.UsuarioAtual != null)
            {
                NomeUsuario = SessaoSistema.UsuarioAtual.Nome;
                CargoUsuario = SessaoSistema.UsuarioAtual.Tipo.ToString();
            }

        }

        // Construtor
        public PrincipalViewModel()
        {
            CarregarUuario();
        }
    }
}
