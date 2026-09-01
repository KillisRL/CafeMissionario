using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CafeMissionario.ViewModels
{
    public partial class ProdutoViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private ObservableCollection<Produto> _listaProdutos = new();  

        [ObservableProperty] public string _textoBusca = string.Empty;

        [ObservableProperty] private string _nome = string.Empty;

        [ObservableProperty] private decimal _preco;

        [ObservableProperty] private int _quantidadeEstoque;

        [ObservableProperty] private bool _controlaEstoque = true;

        // Construtor
        public ProdutoViewModel()
        {
            ConsultarProdutos();
        }

        // Comandos
        [RelayCommand]
        private async Task ExcluirProduto(Produto produto)
        {          
            if (produto == null) return;

            using var db = new AppDbContext();
            bool temVendas = db.ItensPedido.Any(v => v.ProdutoId == produto.Id);

            if (temVendas)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Atenção",
                    $"Não é possível excluir '{produto.Nome}' pois já existem vendas registradas com ele.",
                    "OK");
                return;
            }

            // Pergunta de segurança antes de excluir
            bool confirmar = await Shell.Current.DisplayAlertAsync(
                "Confirmação",
                $"Deseja realmente excluir '{produto.Nome}'?",
                "Sim",
                "Não");

            if (confirmar)
            {
                var produtoParaApagar = db.Produtos.Find(produto.Id);

                if (produtoParaApagar != null)
                {
                    db.Produtos.Remove(produtoParaApagar);
                    await db.SaveChangesAsync();
                
                    ConsultarProdutos();
                }
            }
        }

        [RelayCommand]
        private async Task EditarProduto(Produto produto)
        {
            if (produto == null) return;

            var parametros = new Dictionary<string, object>
            {
                { "ProdutoParaEditar", produto }
            };

            // Navega para a tela de cadastro enviando o objeto selecionado
            await Shell.Current.GoToAsync("ProdutoCadView", parametros);
        }

        // Métodos
        partial void OnTextoBuscaChanged(string? oldValue, string newValue)
        {
            ConsultarProdutos();
        }
        public void ConsultarProdutos()
        {
            using var db = new AppDbContext();

            var lista = string.IsNullOrWhiteSpace(TextoBusca)
                ? db.Produtos.ToList()
                : db.Produtos.Where(p => p.Nome.ToLower().Contains(TextoBusca.ToLower())).ToList();

            ListaProdutos = new ObservableCollection<Produto>(lista);
        }
    }
}
