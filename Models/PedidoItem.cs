using System.ComponentModel.DataAnnotations;

namespace CafeMissionario.Models
{
    public class PedidoItem
    {
        [Key]
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }

    }
}
