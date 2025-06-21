using Microsoft.EntityFrameworkCore;
using APITATT1.Model;

namespace APITATT1.Model
{
    public class Contexto: DbContext
    {
        public Contexto(DbContextOptions<Contexto> options): base(options) { }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Treinador> Treinadores { get; set; }

        public DbSet<User>  Users { get; set; }
        
    }
}
