using System.Net;
using CP5.Application.Dtos;
using CP5.Application.UseCases;
using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CP5.Tests.APP;

public class ClienteUseCaseTest
{
    private readonly Mock<IClienteRepository> _clienteRepository;
    private readonly Mock<IClienteService> _clienteService;
    private readonly ClienteUseCase _clienteUseCase;

    public ClienteUseCaseTest()
    {
        _clienteRepository = new Mock<IClienteRepository>();
        _clienteService = new Mock<IClienteService>();

        _clienteUseCase = new ClienteUseCase(
            _clienteRepository.Object,
            new Mock<ILogger<ClienteUseCase>>().Object,
            _clienteService.Object);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void ObterTodosClientes_DeveRetornarPaginaComLinks()
    {
        // Arrange
        var listaClientes = new List<ClienteEntity>
        {
            new ClienteEntity { Id = 1, Nome = "Carlos", Email = "cliente1@teste.com" },
            new ClienteEntity { Id = 2, Nome = "Andre", Email = "cliente2@teste.com" }
        };

        (IEnumerable<ClienteEntity> data, int rowsCount) pagina = (listaClientes, 2);

        _clienteRepository.Setup(obj => obj.ObterTodos(0, 3)).Returns(pagina);

        // Act
        var resultado = _clienteUseCase.ObterTodosClientes(0, 3);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.OK, resultado.StatusCode);
        Assert.NotNull(resultado.Value);
        Assert.Equal(2, resultado.Value.Items.Count());
        Assert.Equal(2, resultado.Value.TotalRegistros);

        // Links globais da página
        Assert.Contains(resultado.Value.Links, l => l.Rel == "self" && l.Method == "GET");
        Assert.Contains(resultado.Value.Links, l => l.Rel == "create_cliente" && l.Method == "POST");

        // Links de cada item
        var cliente = resultado.Value.Items.Single(x => x.Id == 1);
        Assert.Equal("Carlos", cliente.Nome);
        Assert.Contains(cliente.Links, l => l.Rel == "delete_cliente" && l.Href == "/api/cliente/1");
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void ObterTodosClientes_SemRegistros_DeveRetornar204()
    {
        // Arrange
        _clienteRepository.Setup(obj => obj.ObterTodos(0, 3)).Throws(new NoContentException("Não foi encontrado clientes"));

        // Act
        var resultado = _clienteUseCase.ObterTodosClientes(0, 3);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NoContent, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void ObterTodosClientes_ErroGenerico_DeveRetornar400()
    {
        // Arrange
        _clienteRepository.Setup(obj => obj.ObterTodos(0, 3)).Throws(new Exception("erro"));

        // Act
        var resultado = _clienteUseCase.ObterTodosClientes(0, 3);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.BadRequest, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void ObterUmCliente_DeveRetornarUmClienteComLinks()
    {
        // Arrange
        var cliente = new ClienteEntity { Id = 1, Nome = "Carlos", Email = "cliente1@teste.com" };

        _clienteRepository.Setup(obj => obj.ObterUm(1)).Returns(cliente);

        // Act
        var resultado = _clienteUseCase.ObterUmCliente(1);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
        Assert.Equal(1, resultado.Value.Id);
        Assert.Equal("Carlos", resultado.Value.Nome);
        Assert.Equal(3, resultado.Value.Links.Count);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void ObterUmCliente_NaoEncontrado_DeveRetornar404()
    {
        // Arrange
        _clienteRepository.Setup(obj => obj.ObterUm(99)).Throws(new NoContentException("Não foi encontrado o cliente"));

        // Act
        var resultado = _clienteUseCase.ObterUmCliente(99);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, resultado.StatusCode);
        Assert.Equal("Não foi encontrado o cliente", resultado.Error);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void AdicionarCliente_DeveRetornarClienteAdicionado()
    {
        // Arrange
        var clienteDto = new PostClienteDto { Nome = "Carlos", Email = "cliente1@teste.com", Idade = 10, Estado = "SP" };

        _clienteRepository
            .Setup(obj => obj.Adicionar(It.IsAny<ClienteEntity>()))
            .Returns((ClienteEntity entity) => entity);

        // Act
        var resultado = _clienteUseCase.AdicionarCliente(clienteDto);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
        Assert.Equal(clienteDto.Nome, resultado.Value.Nome);
        Assert.Equal(clienteDto.Email, resultado.Value.Email);
        Assert.Equal(clienteDto.Idade, resultado.Value.Idade);
        Assert.Equal("SP", resultado.Value.Estado!.Nome);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void AdicionarCliente_ErroNoRepository_DeveRetornar400()
    {
        // Arrange
        _clienteRepository.Setup(obj => obj.Adicionar(It.IsAny<ClienteEntity>())).Throws(new Exception("erro"));

        // Act
        var resultado = _clienteUseCase.AdicionarCliente(new PostClienteDto());

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.BadRequest, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void DeletarCliente_NaoEncontrado_DeveRetornar404()
    {
        // Arrange
        _clienteRepository.Setup(obj => obj.Deletar(99)).Throws(new NoContentException("Não foi encontrado o cliente"));

        // Act
        var resultado = _clienteUseCase.DeletarCliente(99);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Clientes")]
    public void ObterTodosClientesMaioresDeIdade_DeveRetornarLista()
    {
        // Arrange
        var lista = new List<ClienteEntity> { new ClienteEntity { Id = 1, Nome = "Carlos", Idade = 20 } };

        _clienteRepository.Setup(obj => obj.ObterTodosMaioresDeIdade(18)).Returns(lista);

        // Act
        var resultado = _clienteUseCase.ObterTodosClientesMaioresDeIdadeClientes(18);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Single(resultado.Value!);
    }

    [Fact]
    [Trait("UseCase", "Servico Externo")]
    public async Task ObterEnderecoPorCep_DeveRetornarEndereco()
    {
        // Arrange
        var endereco = new EnderecoModel { Cep = "01001-000", Logradouro = "Praça da Sé", Bairro = "Sé", Uf = "SP" };

        _clienteService.Setup(obj => obj.ObterEnderecoPorCepAsync("01001000")).ReturnsAsync(endereco);

        // Act
        var resultado = await _clienteUseCase.ObterEnderecoPorCepAsync("01001000");

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.OK, resultado.StatusCode);
        Assert.Equal("SP", resultado.Value!.Uf);
    }

    [Fact]
    [Trait("UseCase", "Servico Externo")]
    public async Task ObterEnderecoPorCep_NaoEncontrado_DeveRetornar404()
    {
        // Arrange
        _clienteService
            .Setup(obj => obj.ObterEnderecoPorCepAsync("00000000"))
            .ThrowsAsync(new NoContentException("Não foi localizado o endereco para esta cep: 00000000"));

        // Act
        var resultado = await _clienteUseCase.ObterEnderecoPorCepAsync("00000000");

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Servico Externo")]
    public async Task ObterEnderecoPorCep_ErroGenerico_DeveRetornar400()
    {
        // Arrange
        _clienteService.Setup(obj => obj.ObterEnderecoPorCepAsync("1")).ThrowsAsync(new Exception("erro"));

        // Act
        var resultado = await _clienteUseCase.ObterEnderecoPorCepAsync("1");

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.BadRequest, resultado.StatusCode);
    }
}
