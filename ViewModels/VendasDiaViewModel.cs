using CafeMissionario.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CafeMissionario.ViewModels
{
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

        // Comandos
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
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlertAsync("Erro ao carregar vendas", ex.Message, "OK");
            }
        }
    }
}