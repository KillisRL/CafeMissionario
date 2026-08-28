using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace CafeMissionario.ViewModels
{
    public partial class ItemCardapio : ObservableObject
    {
        public Produto ProdutoBase { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        [ObservableProperty] private int _quantidadeSelecionada;
    }
    public partial class PedidoViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private ObservableCollection<ItemCardapio> _listaProdutos = new();
        [ObservableProperty] private decimal _valorTotal;
        [ObservableProperty] private string _nomeCliente = string.Empty;
        [ObservableProperty] private string _formaPagamento = string.Empty;
        [ObservableProperty] private string _vendedor = string.Empty;

        public List<string> FormasPagamentoOpcoes { get; } = new()
        {
            "Dinheiro",
            "Cartão de Débito",
            "Cartão de Crédito",
            "PIX"
        };


        // Construtor
        public PedidoViewModel()
        {
            CarregarCardapio();
        }

        // Métodos
        public void CarregarCardapio()
        {
            using var db = new AppDbContext();
            ListaProdutos.Clear();

            var produtosDoBanco = db.Produtos.ToList();

            foreach (var p in produtosDoBanco)
            {
                ListaProdutos.Add(new ItemCardapio
                {
                    ProdutoBase = p,
                    ProdutoId = p.Id,
                    Nome = p.Nome,
                    Preco = p.Preco,
                    QuantidadeSelecionada = 0
                });
            }
        }

        private void CalcularTotal()
        {
            // Soma a (Quantidade * Preço) de todos os itens da lista
            ValorTotal = ListaProdutos.Sum(i => i.QuantidadeSelecionada * i.ProdutoBase.Preco);
        }

        // Comandos
        [RelayCommand]
        private void Adicionar(ItemCardapio item)
        {
            item.QuantidadeSelecionada++;
            CalcularTotal();
        }

        [RelayCommand]
        private void Remover(ItemCardapio item)
        {
            if (item.QuantidadeSelecionada > 0)
            {
                item.QuantidadeSelecionada--;
                CalcularTotal();
            }
        }

        [RelayCommand]
        private async Task SalvarPedido()
        {
            if (string.IsNullOrWhiteSpace(NomeCliente) || string.IsNullOrWhiteSpace(FormaPagamento))
            {
                await Shell.Current.DisplayAlertAsync("Atenção", "Preencha o Nome do Cliente ou Forma de Pagamento", "Ok");
                return;
            }

            var itensComprados = ListaProdutos.Where(i => i.QuantidadeSelecionada > 0).ToList();

            if (!itensComprados.Any())
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "Selecione ao menos um item.", "OK");
                return;
            }

            // Baixar Estoque
            await BaixarEstoqueDoPedido(itensComprados);

            // Texto para copiar e colar no Whatsapp
            string textoParaCopiar = $"*NOVO PEDIDO*\n*Cliente:* {NomeCliente}\n*Forma de Pagamento:* {FormaPagamento}\n*Itens:*\n";

            foreach (var item in itensComprados)
            {
                textoParaCopiar += $"- {item.QuantidadeSelecionada}x {item.ProdutoBase.Nome} (R$ {item.ProdutoBase.Preco:F2})\n";
            }

            textoParaCopiar += $"\n*TOTAL: R$ {ValorTotal:F2}*";

            // Salvar no Banco de Dados
            using (var db = new AppDbContext())
            {

                var vendedor = SessaoSistema.UsuarioAtual.Nome;

                var pedido = new Pedido()
                {
                    NomeCliente = this.NomeCliente,
                    Total = this.ValorTotal,
                    FormaPagamento = this.FormaPagamento,
                    DataHora = DateTime.Now,
                    Vendedor = vendedor

                };

                db.Pedidos.Add(pedido);
                await db.SaveChangesAsync();
            }

            // Copiar texto para área de transferência
            await Clipboard.Default.SetTextAsync(textoParaCopiar);

            await Shell.Current.DisplayAlertAsync("Informação", "Pedido Finalizado e copiado para a área de transferência", "Ok");

            // Limpar a tela
            NomeCliente = string.Empty;
            FormaPagamento = string.Empty;
            CarregarCardapio();
        }

        public async Task BaixarEstoqueDoPedido(List<ItemCardapio> itensVendidos)
        {
            using var db = new AppDbContext();

            foreach (var item in itensVendidos)
            {
                // Busca a receita (Ficha Técnica) deste item
                var receita = db.FichasTecnicas
                                .Where(f => f.ProdutoId == item.ProdutoId)
                                .ToList();

                if (receita.Any())
                {
                    // Se tem ficha técnica, desconta do insumo base (Ex: Pão Francês)
                    foreach (var ingrediente in receita)
                    {
                        var insumoNoBanco = db.Produtos.Find(ingrediente.InsumoId);
                        if (insumoNoBanco != null && insumoNoBanco.ControlaEstoque)
                        {
                            decimal totalAbater = ingrediente.QuantidadeConsumida * item.QuantidadeSelecionada;
                            insumoNoBanco.QuantidadeEstoque -= totalAbater;
                            db.Produtos.Update(insumoNoBanco);
                        }
                    }
                }
                else
                {
                    // Se não tem ficha técnica, desconta do próprio produto se ele controlar estoque
                    var produtoNoBanco = db.Produtos.Find(item.ProdutoId);
                    if (produtoNoBanco != null && produtoNoBanco.ControlaEstoque)
                    {
                        produtoNoBanco.QuantidadeEstoque -= item.QuantidadeSelecionada;
                        db.Produtos.Update(produtoNoBanco);
                    }
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
