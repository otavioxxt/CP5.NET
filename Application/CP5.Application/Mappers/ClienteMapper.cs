using CP5.Application.Dtos;
using CP5.Domain.Entities;

namespace CP5.Application.Mappers;

public static class ClienteMapper
{
    public static ClienteEntity ToClienteEntity(this PostClienteDto obj)
    {
        return new ClienteEntity
        {
            Nome = obj.Nome,
            Email = obj.Email,
            Idade = obj.Idade,
            Estado = new EstadoEntity { Nome = obj.Estado },
        };
    }

    public static GetClienteDto ToClienteDto(this ClienteEntity obj, IEnumerable<LinkDto> links)
    {
        return new GetClienteDto
        {
            Id = obj.Id,
            Nome = obj.Nome,
            Idade = obj.Idade,
            Email = obj.Email,
            Estado = obj.Estado?.Nome ?? string.Empty,
            Produtos = obj.Produtos,
            Cursos = obj.Cursos,
            Links = links?.ToList() ?? Enumerable.Empty<LinkDto>().ToList()
        };
    }
}
