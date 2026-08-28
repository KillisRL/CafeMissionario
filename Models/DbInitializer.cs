using CafeMissionario.Data;
using CafeMissionario.Models;

namespace CafeMissionario.Helpers
{
    public static class DbInitializer
    {
        public static void Init()
        {
            using var db = new AppDbContext();

            db.Database.EnsureCreated();

            if (!db.Usuarios.Any())
            {
                db.Usuarios.Add(new Usuario
                {
                    Nome = "Administrador",
                    Senha = "123Luiz!2809",
                    Tipo = UsuarioTipo.Administrador
                });

                db.SaveChanges();
            }
        }
    }
}