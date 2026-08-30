using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;

namespace CafeMissionario.ViewModels
{
    public partial class ItemCardapio : ObservableObject
    {
        public Produto ProdutoBase { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public decimal QuantidadeEstoque { get; set; }
        public bool ControlaEstoque { get; set; }
        [ObservableProperty] public bool _exibirInsumo = true;
        [ObservableProperty] public bool _exibirMsg = true;
        [ObservableProperty] public string _insumo = string.Empty;
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

        #region Metodos
        // Métodos

        #region CarregarCardapio
        public void CarregarCardapio()
        {
            using var db = new AppDbContext();
            ListaProdutos.Clear();

            var produtosDoBanco = db.Produtos.ToList();

            foreach (var p in produtosDoBanco)
            {
                // Buscar Ficha Técnica (PONTO DE ATENÇÃO PARA CORREÇÃO FUTURA)
                var receita = db.FichasTecnicas
                    .Where(f => f.ProdutoId == p.Id)
                    .ToList();
                string nomeInsumo = string.Empty;
                foreach (var insumo in receita)
                {
                    var insumoNoBanco = db.Produtos.Find(insumo.InsumoId);
                    if (insumoNoBanco != null && insumoNoBanco.ControlaEstoque)
                    {
                        nomeInsumo = insumoNoBanco.Nome;
                    }
                }

                if (p.ControlaEstoque == true)
                {
                    ListaProdutos.Add(new ItemCardapio
                    {
                        ProdutoBase = p,
                        ProdutoId = p.Id,
                        Nome = p.Nome,
                        Preco = p.Preco,
                        QuantidadeEstoque = p.QuantidadeEstoque,
                        ExibirInsumo = false,
                        ExibirMsg = false,
                        QuantidadeSelecionada = 0
                    });
                }
                else if (p.ControlaEstoque != true && !string.IsNullOrEmpty(nomeInsumo.Trim()))
                {
                    ListaProdutos.Add(new ItemCardapio
                    {
                        ProdutoBase = p,
                        ProdutoId = p.Id,
                        Nome = p.Nome,
                        Preco = p.Preco,
                        QuantidadeEstoque = p.QuantidadeEstoque,
                        ExibirInsumo = true,
                        ExibirMsg = false,
                        Insumo = nomeInsumo,
                        QuantidadeSelecionada = 0
                    });
                }
                else
                {
                    ListaProdutos.Add(new ItemCardapio
                    {
                        ProdutoBase = p,
                        ProdutoId = p.Id,
                        Nome = p.Nome,
                        Preco = p.Preco,
                        QuantidadeEstoque = p.QuantidadeEstoque,
                        ExibirInsumo = false,
                        ExibirMsg = true,
                        QuantidadeSelecionada = 0
                    });
                }
            }
        }
        #endregion

        private void CalcularTotal()
        {
            // Soma a (Quantidade * Preço) de todos os itens da lista
            ValorTotal = ListaProdutos.Sum(i => i.QuantidadeSelecionada * i.ProdutoBase.Preco);
        }

        #region ValidarEstoqueCarrinho
        private bool ValidarEstoqueCarrinho(ItemCardapio itemDesejado)
        {
            using var db = new AppDbContext();

            // 1. Descobrir o que o clique atual vai consumir (Lista de Necessidades)
            // Usamos um Dicionário: Key = ID do Insumo, Value = Quantidade Necessária
            var necessidadesDesteClique = new Dictionary<int, decimal>();

            var receitaDoClique = db.FichasTecnicas
                                    .Where(f => f.ProdutoId == itemDesejado.ProdutoId)
                                    .ToList();

            if (receitaDoClique.Any())
            {
                // Se é PRODUTO FABRICADO, adiciona todos os ingredientes da receita dele
                foreach (var ing in receitaDoClique)
                {
                    necessidadesDesteClique.Add(ing.InsumoId, ing.QuantidadeConsumida);
                }
            }
            else
            {
                // Se é VENDA DIRETA, o produto consome a si mesmo (1 unidade)
                necessidadesDesteClique.Add(itemDesejado.ProdutoId, 1);
            }

            // 2. Otimização: Carregar as receitas dos itens que já estão no carrinho de uma vez só
            var itensNoCarrinho = ListaProdutos.Where(i => i.QuantidadeSelecionada > 0).ToList();
            var idsNoCarrinho = itensNoCarrinho.Select(i => i.ProdutoId).ToList();
            var receitasDoCarrinho = db.FichasTecnicas
                                       .Where(f => idsNoCarrinho.Contains(f.ProdutoId))
                                       .ToList();

            // 3. Validar se o estoque aguenta a soma do (Carrinho + Clique Atual)
            foreach (var necessidade in necessidadesDesteClique)
            {
                int insumoId = necessidade.Key;
                decimal qtdNecessariaParaUm = necessidade.Value;

                var insumoNoBanco = db.Produtos.Find(insumoId);

                // Só fazemos a barreira se o produto estiver marcado para controlar estoque
                if (insumoNoBanco != null && insumoNoBanco.ControlaEstoque)
                {
                    decimal totalJaNoCarrinho = 0;

                    foreach (var itemCar in itensNoCarrinho)
                    {
                        // Busca a receita do item que está no carrinho
                        var receitaDesteItem = receitasDoCarrinho.Where(f => f.ProdutoId == itemCar.ProdutoId).ToList();

                        if (receitaDesteItem.Any())
                        {
                            // Se o item do carrinho é fabricado, verifica se ele USA o insumo que estamos validando
                            var uso = receitaDesteItem.FirstOrDefault(r => r.InsumoId == insumoId);
                            if (uso != null)
                            {
                                totalJaNoCarrinho += (uso.QuantidadeConsumida * itemCar.QuantidadeSelecionada);
                            }
                        }
                        else
                        {
                            // Se o item do carrinho é venda direta, verifica se ele É o próprio insumo
                            if (itemCar.ProdutoId == insumoId)
                            {
                                totalJaNoCarrinho += itemCar.QuantidadeSelecionada;
                            }
                        }
                    }

                    // A MÁGICA ACONTECE AQUI: Soma o consumo indireto + direto + clique
                    decimal necessidadeFutura = totalJaNoCarrinho + qtdNecessariaParaUm;

                    // Se estourar o banco, bloqueia na hora!
                    if (necessidadeFutura > insumoNoBanco.QuantidadeEstoque)
                    {
                        return false;
                    }
                }
            }

            return true; // Passou em todas as validações de todos os insumos
        }
        #endregion

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

        #endregion


        // Comandos
        [RelayCommand]
        private async Task Adicionar(ItemCardapio item)
        {
            // Chama a regra de negócio para validar o estoque real
            bool temEstoque = ValidarEstoqueCarrinho(item);

            if (temEstoque)
            {
                item.QuantidadeSelecionada++;
                CalcularTotal();
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Estoque Insuficiente",
                    $"Não há insumos suficientes para preparar mais um(a) {item.Nome}.", "OK");
            }
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

        #region SalvarPedido
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

                var vendedor = SessaoSistema.UsuarioAtual?.Nome ?? "Visitante";

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

                foreach (var item in itensComprados)
                {
                    var pedidoItem = new PedidoItem()
                    {
                        PedidoId = pedido.Id,
                        NomeProduto = item.ProdutoBase.Nome,
                        Quantidade = item.QuantidadeSelecionada,
                        Preco = item.ProdutoBase.Preco
                    };

                    db.ItensPedido.Add(pedidoItem);
                }

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
        #endregion


    }
}
