using CommunityToolkit.Mvvm.ComponentModel;

namespace CafeMissionario.Models
{
    public partial class IngredienteItem : ObservableObject
    {
        public int InsumoId { get; set; }
        public string NomeInsumo { get; set; } = string.Empty;

        [ObservableProperty]
        private decimal _quantidadeConsumida;
    }
}