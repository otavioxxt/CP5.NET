using CP5.Application.Dtos;
using CP5.Application.Mappers;
using CP5.Domain.Entities;
using CP5.Domain.Errors;

namespace CP5.Tests.APP;

// Testes de unidade do Domain e dos Mappers
public class EntityTest
{
    [Fact]
    [Trait("Domain", "OperationResult")]
    public void OperationResult_Success_DeveTerValorEStatus()
    {
        var resultado = OperationResult<string>.Success("ok", 201);

        Assert.True(resultado.IsSuccess);
        Assert.Equal("ok", resultado.Value);
        Assert.Equal(201, resultado.StatusCode);
        Assert.Null(resultado.Error);
    }

    [Fact]
    [Trait("Domain", "OperationResult")]
    public void OperationResult_Failure_DeveTerErroEStatusPadrao400()
    {
        var resultado = OperationResult<string>.Failure("falhou");

        Assert.False(resultado.IsSuccess);
        Assert.Equal("falhou", resultado.Error);
        Assert.Equal(400, resultado.StatusCode);
        Assert.Null(resultado.Value);
    }

    [Fact]
    [Trait("Domain", "Erros")]
    public void NoContentException_DeveCarregarMensagem()
    {
        var ex = new NoContentException("Não foi encontrado o cliente");

        Assert.Equal("Não foi encontrado o cliente", ex.Message);
        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    [Trait("Domain", "Hateoas")]
    public void LinkDto_DeveUsarGetComoMetodoPadrao()
    {
        var link = new LinkDto("/api/cliente", "self");

        Assert.Equal("/api/cliente", link.Href);
        Assert.Equal("self", link.Rel);
        Assert.Equal("GET", link.Method);
    }

    [Fact]
    [Trait("Domain", "Paginacao")]
    public void PageResponseDto_DeveIniciarVazio()
    {
        var pagina = new PageResponseDto<GetClienteDto>();

        Assert.Empty(pagina.Items);
        Assert.Empty(pagina.Links);
        Assert.Equal(0, pagina.TotalRegistros);
    }

    [Fact]
    [Trait("Mapper", "Cliente")]
    public void ToClienteEntity_DeveConverterDto()
    {
        var dto = new PostClienteDto { Nome = "João", Email = "joao@teste.com", Idade = 30, Estado = "SP" };

        var entity = dto.ToClienteEntity();

        Assert.Equal("João", entity.Nome);
        Assert.Equal("joao@teste.com", entity.Email);
        Assert.Equal(30, entity.Idade);
        Assert.Equal("SP", entity.Estado!.Nome);
    }

    [Fact]
    [Trait("Mapper", "Cliente")]
    public void ToClienteDto_DeveConverterEntityComLinks()
    {
        var entity = new ClienteEntity
        {
            Id = 1,
            Nome = "João",
            Email = "joao@teste.com",
            Idade = 30,
            Estado = new EstadoEntity { Nome = "SP" }
        };

        var dto = entity.ToClienteDto(new List<LinkDto> { new LinkDto("/api/cliente/1", "get_um_cliente") });

        Assert.Equal(1, dto.Id);
        Assert.Equal("SP", dto.Estado);
        Assert.Single(dto.Links);
    }

    [Fact]
    [Trait("Mapper", "Produto e Curso")]
    public void ToProdutoEntity_E_ToCursoEntity_DeveConverterDtos()
    {
        Assert.Equal("Mouse", new PostProdutoDto { Nome = "Mouse" }.ToProdutoEntity().Nome);
        Assert.Equal("C#", new PostCursoDto { Nome = "C#" }.ToCursoEntity().Nome);
    }

    [Fact]
    [Trait("Mapper", "Usuario")]
    public void ToUserEntity_DeveConverterDto()
    {
        var dto = new PostUserDto { Nome = "Admin", Email = "admin@fiap.com.br", Password = "123", ConfirmPassword = "123", Role = "admin" };

        var entity = dto.ToUserEntity();

        Assert.Equal("admin@fiap.com.br", entity.Email);
        Assert.Equal("123", entity.Password);
        Assert.Equal("admin", entity.Role);
    }
}
