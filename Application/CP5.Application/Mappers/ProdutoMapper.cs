using CP5.Application.Dtos;
using CP5.Domain.Entities;

namespace CP5.Application.Mappers;

public static class ProdutoMapper
{
    public static ProdutoEntity ToProdutoEntity(this PostProdutoDto obj)
    {
        return new ProdutoEntity { Nome = obj.Nome };
    }
}
