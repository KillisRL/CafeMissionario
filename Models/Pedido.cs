using System.ComponentModel.DataAnnotations;

namespace CafeMissionario.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string Vendedor { get; set; } = string.Empty;
    }
}
