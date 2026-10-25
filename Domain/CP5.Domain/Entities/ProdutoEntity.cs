using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP5.Domain.Entities;

[Table("tb_cp5_produto")]
public class ProdutoEntity
{
    [Key]
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int ClienteId { get; set; }
}
