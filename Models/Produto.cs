using System.ComponentModel.DataAnnotations;

namespace CafeMissionario.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }
        public bool ControlaEstoque { get; set; } = true;

        public decimal QuantidadeEstoque { get; set; }
    }
}
