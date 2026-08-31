using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace CafeMissionario.ViewModels
{
    public class PagamentoResumo
    {
        public string FormaPagamento { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
    public class ProdutoVendidoResumo
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
    public class PedidoDiaItem
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Vendedor { get; set; } = string.Empty;
        public string FormaPagamento { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string HoraFormatada => DataHora.ToString("HH:mm");
        public DateTime DataHora { get; set; }    
    }

    public partial class VendasDiaViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private ObservableCollection<PedidoDiaItem> _vendasHoje = new();
        [ObservableProperty] private decimal _totalAcumuladoHoje;
        [ObservableProperty] private ObservableCollection<ProdutoVendidoResumo> _resumoProdutos = new();
        [ObservableProperty] private int _totalItensVendidos;
        [ObservableProperty] private ObservableCollection<PagamentoResumo> _resumoPagamentos = new();

        // Construtor
        public VendasDiaViewModel()
        {

        }

        // Comandos
        #region CarregarVendasHoje
        [RelayCommand]
        public void CarregarVendasHoje()
        {
            try
            {
                using var db = new AppDbContext();

                // Define o intervalo das 00:00:00 até as 23:59:59 de hoje
                var inicioDia = DateTime.Today;
                var fimDia = DateTime.Today.AddDays(1).AddTicks(-1);

                var pedidos = db.Pedidos
                    .Where(p => p.DataHora >= inicioDia && p.DataHora <= fimDia)
                    .OrderByDescending(p => p.DataHora)
                    .ToList();

                VendasHoje.Clear();
                foreach (var p in pedidos)
                {
                    VendasHoje.Add(new PedidoDiaItem
                    {
                        Id = p.Id,
                        Cliente = string.IsNullOrWhiteSpace(p.NomeCliente) ? "Consumidor" : p.NomeCliente,
                        Vendedor = string.IsNullOrWhiteSpace(p.Vendedor) ? "Atendente" : p.Vendedor,
                        FormaPagamento = p.FormaPagamento,
                        Total = p.Total,
                        DataHora = p.DataHora
                    });
                }

                TotalAcumuladoHoje = VendasHoje.Sum(v => v.Total);

                var itensAgrupados = db.ItensPedido
                    .Where(i => i.Pedido.DataHora >= inicioDia && i.Pedido.DataHora <= fimDia)
                    .GroupBy(i => i.NomeProduto)
                    .Select(g => new ProdutoVendidoResumo
                    {
                        NomeProduto = g.Key,
                        Quantidade = g.Sum(i => i.Quantidade)
                    })
                    .OrderByDescending(r => r.Quantidade)
                    .ToList();

                ResumoProdutos.Clear();
                foreach (var item in itensAgrupados)
                {
                    ResumoProdutos.Add(item);
                }

                TotalItensVendidos = ResumoProdutos.Sum(r => r.Quantidade);

                // AGRUPAMENTO POR FORMA DE PAGAMENTO
                var pagamentosAgrupados = pedidos
                    .GroupBy(p => p.FormaPagamento)
                    .Select(g => new PagamentoResumo
                    {
                        FormaPagamento = g.Key,
                        Total = g.Sum(p => p.Total)
                    })
                    .OrderByDescending(r => r.Total)
                    .ToList();

                ResumoPagamentos.Clear();
                foreach (var pag in pagamentosAgrupados)
                {
                    ResumoPagamentos.Add(pag);
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlertAsync("Erro ao carregar vendas", ex.Message, "OK");
            }
        }
        #endregion

        #region Exportar Relatorio
        [RelayCommand]
        public async Task ExportarRelatorio()
        {
            try
            {
                // 1. Monta o texto do relatório (estilo cupom fiscal / gerencial)
                var sb = new StringBuilder();
                sb.AppendLine("===================================");
                sb.AppendLine("       FECHAMENTO DE CAIXA         ");
                sb.AppendLine("         CAFÉ MISSIONÁRIO          ");
                sb.AppendLine("===================================");
                sb.AppendLine($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}");
                sb.AppendLine("-----------------------------------");
                sb.AppendLine($"TOTAL VENDIDO: R$ {TotalAcumuladoHoje:F2}");
                sb.AppendLine($"QTD DE VENDAS: {VendasHoje.Count} vendas");
                sb.AppendLine($"TOTAL DE ITENS: {TotalItensVendidos} unidades");
                sb.AppendLine("-----------------------------------");
                sb.AppendLine("        RESUMO DE PRODUTOS         ");
                sb.AppendLine("-----------------------------------");
                sb.AppendLine("-----------------------------------");
                sb.AppendLine("       VENDAS POR PAGAMENTO        ");
                sb.AppendLine("-----------------------------------");
                foreach (var pag in ResumoPagamentos)
                {
                    sb.AppendLine($"{pag.FormaPagamento}: R$ {pag.Total:F2}");
                }

                foreach (var item in ResumoProdutos)
                {
                    // Formata bonitinho: Ex: "15x Pão Francês"
                    sb.AppendLine($"{item.Quantidade}x {item.NomeProduto}");
                }

                sb.AppendLine("===================================");

                // 2. Cria um arquivo temporário no dispositivo (Windows ou Android)
                string nomeArquivo = $"Fechamento_Caixa_{DateTime.Now:dd-MM-yyyy}.txt";
                string caminhoArquivo = Path.Combine(FileSystem.CacheDirectory, nomeArquivo);

                // Escreve o texto no arquivo
                File.WriteAllText(caminhoArquivo, sb.ToString());

                // 3. Chama o compartilhamento nativo do sistema
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartilhar Fechamento de Caixa",
                    File = new ShareFile(caminhoArquivo)
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível exportar: {ex.Message}", "OK");
            }
        }
        #endregion

        //[RelayCommand]
        //public async Task CancelarVenda(Pedido pedido)
        //{
        //    if (pedido == null) return;

        //    // Pergunta de segurança antes de apagar
        //    bool confirmar = await Shell.Current.DisplayAlertAsync(
        //        "Confirmação",
        //        $"Deseja realmente excluir o pedido de '{pedido.NomeCliente}'?",
        //        "Sim",
        //        "Não");

        //    if (confirmar)
        //    {
        //        using var db = new AppDbContext();
        //        db.Pedidos.Remove(pedido);
        //        await db.SaveChangesAsync();

        //        // Recarrega a lista para o item sumir da tela
        //        CarregarVendasHoje();
        //    }

        //}

        [RelayCommand]
        private async Task EditarVenda(PedidoDiaItem pedido)
        {
            if (pedido == null) return;

            using var db = new AppDbContext();
            var pedidoSalvo = db.Pedidos.Find(pedido.Id);
            if (pedidoSalvo == null) return;
            var parametros = new Dictionary<string, object>
            {
                { "VendaParaEditar", pedidoSalvo }
            };

            // Navega para a tela de cadastro enviando o objeto selecionado
            await Shell.Current.GoToAsync("PedidoView", parametros);
        }
    }
}