using CP5.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CP5.Infrastructure.Data.AppData;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Usuario { get; set; }
    public DbSet<ClienteEntity> Cliente { get; set; }
    public DbSet<ProdutoEntity> Produto { get; set; }
    public DbSet<CursoEntity> Curso { get; set; }
    public DbSet<EstadoEntity> Estado { get; set; }
}
