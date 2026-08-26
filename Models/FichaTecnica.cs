using System;
using System.Collections.Generic;
using System.Text;

namespace CafeMissionario.Models
{
    public class FichaTecnica
    {
        public int Id { get; set; }

        // O produto final vendido no cardápio (Ex: Pão com Nutella)
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

        // O item base que realmente tem estoque controlado (Ex: Pão Francês)
        public int InsumoId { get; set; }
        public Produto Insumo { get; set; }

        public decimal QuantidadeConsumida { get; set; }
    }
}
