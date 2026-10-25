using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Infrastructure.Data.AppData;
using CP5.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CP5.Tests.APP;

// Testes de unidade do Repository executados no banco da aplicação (Oracle)
public class ClienteRepositoryTest : IDisposable
{
    private readonly ApplicationContext _applicationContext;
    private readonly ClienteRepository _clienteRepository;

    private readonly List<int> _idsClientes = new();
    private readonly List<int> _idsEstados = new();

    public ClienteRepositoryTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseOracle(configuration.GetConnectionString("Oracle"))
            .Options;

        _applicationContext = new ApplicationContext(options);
        _clienteRepository = new ClienteRepository(_applicationContext);
    }

    private ClienteEntity CriarCliente(string nome = "João", int idade = 30)
    {
        var cliente = new ClienteEntity
        {
            Nome = nome,
            Email = $"{Guid.NewGuid():N}@teste.com",
            Idade = idade,
            Estado = new EstadoEntity { Nome = "SP" }
        };

        var resultado = _clienteRepository.Adicionar(cliente)!;

        _idsClientes.Add(resultado.Id);
        if (resultado.EstadoId.HasValue) _idsEstados.Add(resultado.EstadoId.Value);

        return resultado;
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void Adicionar_DeveAdicionarCliente()
    {
        // Arrange / Act
        var resultado = CriarCliente("João", 30);

        // Assert
        var clienteNoDb = _applicationContext.Cliente.FirstOrDefault(c => c.Id == resultado.Id);

        Assert.NotNull(clienteNoDb);
        Assert.Equal("João", clienteNoDb.Nome);
        Assert.Equal(resultado.Email, clienteNoDb.Email);
        Assert.Equal(30, clienteNoDb.Idade);
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void ObterUm_DeveRetornarUmCliente()
    {
        // Arrange
        var cliente = CriarCliente("Maria", 25);

        // Act
        var resultado = _clienteRepository.ObterUm(cliente.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(cliente.Id, resultado.Id);
        Assert.Equal("Maria", resultado.Nome);
        Assert.Equal("SP", resultado.Estado!.Nome);
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void ObterUm_ClienteInexistente_DeveLancarNoContentException()
    {
        Assert.Throws<NoContentException>(() => _clienteRepository.ObterUm(-1));
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void ObterTodos_DeveRetornarPaginaComTotalDeRegistros()
    {
        // Arrange
        CriarCliente("João", 30);
        CriarCliente("Pedro", 40);

        // Act
        var resultado = _clienteRepository.ObterTodos(0, 10);

        // Assert
        Assert.NotEmpty(resultado.data);
        Assert.True(resultado.data.Count() <= 10);
        Assert.True(resultado.rowsCount >= 2);
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void ObterTodosMaioresDeIdade_DeveRetornarSomenteMaiores()
    {
        // Arrange
        var cliente = CriarCliente("Idoso", 150);

        // Act
        var resultado = _clienteRepository.ObterTodosMaioresDeIdade(150);

        // Assert
        Assert.Contains(resultado, c => c.Id == cliente.Id);
        Assert.All(resultado, c => Assert.True(c.Idade >= 150));
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void ObterTodosMaioresDeIdade_SemResultado_DeveLancarNoContentException()
    {
        Assert.Throws<NoContentException>(() => _clienteRepository.ObterTodosMaioresDeIdade(int.MaxValue));
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void Editar_DeveAlterarOsDadosDoCliente()
    {
        // Arrange
        var cliente = CriarCliente("João", 30);

        var novosDados = new ClienteEntity
        {
            Nome = "João Editado",
            Email = $"{Guid.NewGuid():N}@teste.com",
            Idade = 41,
            Estado = new EstadoEntity { Nome = "RJ" }
        };

        // Act
        var resultado = _clienteRepository.Editar(cliente.Id, novosDados);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("João Editado", resultado.Nome);
        Assert.Equal(41, resultado.Idade);
        Assert.Equal("RJ", resultado.Estado!.Nome);
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void Editar_ClienteInexistente_DeveLancarNoContentException()
    {
        Assert.Throws<NoContentException>(() => _clienteRepository.Editar(-1, new ClienteEntity()));
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void Deletar_DeveRemoverCliente()
    {
        // Arrange
        var cliente = CriarCliente("Para Deletar", 20);

        // Act
        var resultado = _clienteRepository.Deletar(cliente.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Null(_applicationContext.Cliente.FirstOrDefault(c => c.Id == cliente.Id));
    }

    [Fact]
    [Trait("Repository", "Clientes")]
    public void Deletar_ClienteInexistente_DeveLancarNoContentException()
    {
        Assert.Throws<NoContentException>(() => _clienteRepository.Deletar(-1));
    }

    [Fact]
    [Trait("Repository", "Produtos")]
    public void AdicionarProduto_DeveVincularProdutoAoCliente()
    {
        // Arrange
        var cliente = CriarCliente();

        // Act
        var produto = _clienteRepository.AdicionarProduto(cliente.Id, new ProdutoEntity { Nome = "Mouse" });

        // Assert
        Assert.NotNull(produto);
        Assert.True(produto.Id > 0);
        Assert.Equal(cliente.Id, produto.ClienteId);
    }

    [Fact]
    [Trait("Repository", "Produtos")]
    public void EditarProduto_DeveAlterarNomeDoProduto()
    {
        // Arrange
        var cliente = CriarCliente();
        var produto = _clienteRepository.AdicionarProduto(cliente.Id, new ProdutoEntity { Nome = "Mouse" })!;

        // Act
        var resultado = _clienteRepository.EditarProduto(produto.Id, cliente.Id, new ProdutoEntity { Nome = "Teclado" });

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Teclado", resultado.Nome);
    }

    [Fact]
    [Trait("Repository", "Produtos")]
    public void AdicionarProduto_ClienteInexistente_DeveLancarNoContentException()
    {
        Assert.Throws<NoContentException>(() => _clienteRepository.AdicionarProduto(-1, new ProdutoEntity { Nome = "Mouse" }));
    }

    [Fact]
    [Trait("Repository", "Cursos")]
    public void AdicionarCurso_DeveVincularCursoAoCliente()
    {
        // Arrange
        var cliente = CriarCliente();

        // Act
        var curso = _clienteRepository.AdicionarCurso(cliente.Id, new CursoEntity { Nome = "C#" });

        // Assert
        Assert.NotNull(curso);
        Assert.True(curso.Id > 0);
        Assert.Equal(cliente.Id, curso.ClienteId);
    }

    [Fact]
    [Trait("Repository", "Cursos")]
    public void EditarCurso_DeveAlterarNomeDoCurso()
    {
        // Arrange
        var cliente = CriarCliente();
        var curso = _clienteRepository.AdicionarCurso(cliente.Id, new CursoEntity { Nome = "C#" })!;

        // Act
        var resultado = _clienteRepository.EditarCurso(curso.Id, new CursoEntity { Nome = "ASP.NET" });

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("ASP.NET", resultado.Nome);
    }

    // Remove do Oracle os registros criados pelos testes
    public void Dispose()
    {
        var clientes = _applicationContext.Cliente.Where(x => _idsClientes.Contains(x.Id)).ToList();
        _applicationContext.Cliente.RemoveRange(clientes);
        _applicationContext.SaveChanges();

        var estados = _applicationContext.Estado.Where(x => _idsEstados.Contains(x.Id)).ToList();
        _applicationContext.Estado.RemoveRange(estados);
        _applicationContext.SaveChanges();

        _applicationContext.Dispose();
    }
}
