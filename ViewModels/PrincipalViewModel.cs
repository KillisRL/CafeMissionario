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
        [ObservableProperty] private string _nomeUsuario = "Xicrinha";
        [ObservableProperty] private string _cargoUsuario = "Visitante";
        [ObservableProperty] private bool _isAdm = false;
        [ObservableProperty] private bool _isColaborador = false;

        // Métodos
        public void CarregarUuario()
        {
            if (SessaoSistema.UsuarioAtual != null)
            {
                NomeUsuario = SessaoSistema.UsuarioAtual.Nome;
                CargoUsuario = SessaoSistema.UsuarioAtual.Tipo.ToString();
            }

        }

        public void ValidarPermissao()
        {
            try
            {
                if (SessaoSistema.UsuarioAtual != null)
                {
                    IsAdm = (SessaoSistema.UsuarioAtual.Tipo.ToString() ==
                        UsuarioTipo.Administrador.ToString());

                    IsColaborador = (SessaoSistema.UsuarioAtual.Tipo.ToString() !=
                        UsuarioTipo.Administrador.ToString());
                }
                else
                {
                    IsAdm = false;
                    IsColaborador = false;
                }
            }
            catch
            {
                Shell.Current.DisplayAlertAsync("Atenção", "Não foi encontrado o tipo de usuário", "Ok");
            }
        }

        // Construtor
        public PrincipalViewModel()
        {
            CarregarUuario();
            ValidarPermissao();
        }
    }
}
