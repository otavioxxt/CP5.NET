using CP5.Application.Dtos;
using CP5.Domain.Entities;

namespace CP5.Application.Mappers;

public static class CursoMapper
{
    public static CursoEntity ToCursoEntity(this PostCursoDto obj)
    {
        return new CursoEntity { Nome = obj.Nome };
    }
}
