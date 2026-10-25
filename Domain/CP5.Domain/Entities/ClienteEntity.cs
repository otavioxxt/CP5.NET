using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CP5.Domain.Entities;

[Table("tb_cp5_cliente")]
[Index(nameof(Email), nameof(Nome))] // Indice Composto
[Index(nameof(Email), IsUnique = true, Name = "IDX_cp5_cliente_email")]
public class ClienteEntity
{
    [Key]
    public int Id { get; set; }

    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    public int Idade { get; set; }

    public int? EstadoId { get; set; }
    public EstadoEntity? Estado { get; set; }

    public ICollection<ProdutoEntity>? Produtos { get; set; } = new List<ProdutoEntity>();
    public ICollection<CursoEntity>? Cursos { get; set; } = new List<CursoEntity>();
}
