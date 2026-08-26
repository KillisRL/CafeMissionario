using Microsoft.EntityFrameworkCore;

namespace CafeMissionario.Data
{
    public class AppDbContext : DbContext
    {
        // LISTA DAS TABELAS DO BANCO DE DADOS
        public DbSet<Models.Produto> Produtos { get; set; }
        public DbSet<Models.Pedido> Pedidos { get; set; }
        public DbSet<Models.PedidoItem> ItensPedido { get; set; }
        public DbSet<Models.Usuario> Usuarios { get; set; }
        public DbSet<Models.FichaTecnica> FichasTecnicas { get; set; }
        public AppDbContext()
        {
            // Verificar se o arquivo do banco existe. 
            // Se não existir, cria o arquivo e gera todas as tabelas.
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Local onde o arquivo SQLite será salvo. 
            // FileSystem.AppDataDirectory aponta para uma pasta segura do sistema.
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "banco_cafe.db");

            // Informa ao Entity Framework para usar o SQLite neste caminho
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}