using CafeMissionario.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CafeMissionario.ViewModels
{
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
        [ObservableProperty] private ObservableCollection<PedidoDiaItem> _vendasHoje = new();
        [ObservableProperty] private decimal _totalAcumuladoHoje;

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
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlertAsync("Erro ao carregar vendas", ex.Message, "OK");
            }
        }
    }
}