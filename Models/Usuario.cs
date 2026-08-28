using System.ComponentModel.DataAnnotations;

namespace CafeMissionario.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public UsuarioTipo Tipo { get; set; }
    }
}
