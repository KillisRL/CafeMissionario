using CafeMissionario.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace CafeMissionario.ViewModels
{
    public class VendaPorFormaPagamento
    {
        public string FormaPagamento { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public partial class RelatoriosViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private DateTime _dataFiltro = DateTime.Today;
        [ObservableProperty] private decimal _faturamentoTotal;
        [ObservableProperty] private int _totalPedidos;
        [ObservableProperty] private List<VendaPorFormaPagamento> _resumoPagamentos = new();

        // Construtor
        public RelatoriosViewModel()
        {
            CarregarRelatório();
        }

        // Métodos
        partial void OnDataFiltroChanged(DateTime value)
        {
            CarregarRelatório();
        }

        // Comandos
        [RelayCommand]
        public void CarregarRelatório()
        {
            using var db = new AppDbContext();

            // Define o intervalo de 00:00:00 até 23:59:59 da data selecionada
            var inicioDia = DataFiltro.Date;
            var fimDia = DataFiltro.Date.AddDays(1).AddTicks(-1);

            var pedidosDoDia = db.Pedidos
                .Where(p => p.DataHora >= inicioDia && p.DataHora <= fimDia)
                .ToList();

            TotalPedidos = pedidosDoDia.Count;
            FaturamentoTotal = pedidosDoDia.Sum(p => p.Total);


            ResumoPagamentos = pedidosDoDia
                .GroupBy(p => p.FormaPagamento)
                .Select(g => new VendaPorFormaPagamento
                {
                    FormaPagamento = string.IsNullOrEmpty(g.Key) ? "Não Informado" : g.Key,
                    Total = g.Sum(p => p.Total)
                })
                .ToList();
        }
    }
}