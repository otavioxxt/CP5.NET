using CP5.Domain.Entities;

namespace CP5.Application.Dtos;

public class GetClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Idade { get; set; }
    public string Estado { get; set; } = string.Empty;

    public IEnumerable<ProdutoEntity>? Produtos { get; set; }

    public IEnumerable<CursoEntity>? Cursos { get; set; } = [];

    public List<LinkDto> Links { get; set; } = new();
}
