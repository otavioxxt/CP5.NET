using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using CP5.Application.Dtos;
using CP5.Application.Interfaces;
using CP5.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace CP5.Tests.APP;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IClienteUseCase> ClienteUseCaseMock { get; } = new();
    public Mock<IUserUseCase> UserUseCaseMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Substitui os UseCases pelos Mocks
            services.RemoveAll(typeof(IClienteUseCase));
            services.AddSingleton(ClienteUseCaseMock.Object);

            services.RemoveAll(typeof(IUserUseCase));
            services.AddSingleton(UserUseCaseMock.Object);
        });
    }
}

// Testes funcionais (Controller -> pipeline HTTP real, com Autenticação/Autorização)
public class ClienteControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ClienteControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private string GerarToken(string role)
    {
        var secret = _factory.Services.GetRequiredService<IConfiguration>()["Secretkey"]!;
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "teste@fiap.com.br"),
                new Claim(ClaimTypes.Role, role),
            })
        });

        return tokenHandler.WriteToken(token);
    }

    private HttpClient CriarClienteAutenticado(string role)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GerarToken(role));
        return client;
    }

    [Fact(DisplayName = "GET /api/cliente sem token retorna 401")]
    [Trait("Controller", "Autenticacao")]
    public async Task Get_SemToken_DeveRetornar401()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/cliente");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/cliente com token inválido retorna 401")]
    [Trait("Controller", "Autenticacao")]
    public async Task Get_TokenInvalido_DeveRetornar401()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token.invalido.aqui");

        var response = await client.GetAsync("/api/cliente");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/cliente com token retorna 200 com PageResult e HATEOAS")]
    [Trait("Controller", "Clientes")]
    public async Task Get_ComToken_DeveRetornar200ComLinks()
    {
        // Arrange
        var pagina = new PageResponseDto<GetClienteDto>
        {
            Items = new List<GetClienteDto>
            {
                new GetClienteDto
                {
                    Id = 1,
                    Nome = "João",
                    Links = new List<LinkDto> { new LinkDto("/api/cliente/1", "get_um_cliente") }
                }
            },
            Deslocamento = 0,
            RegistroRetornado = 3,
            TotalRegistros = 1,
            Links = new List<LinkDto>
            {
                new LinkDto("/api/cliente?Deslocamento=0&RegistroRetornado=3", "self"),
                new LinkDto("/api/cliente", "create_cliente", "POST")
            }
        };

        _factory.ClienteUseCaseMock
            .Setup(x => x.ObterTodosClientes(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(OperationResult<PageResponseDto<GetClienteDto>>.Success(pagina, 200));

        using var client = CriarClienteAutenticado("user");

        // Act
        var response = await client.GetAsync("/api/cliente?Deslocamento=0&RegistroRetornado=3");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("totalRegistros", body);
        Assert.Contains("create_cliente", body);
        Assert.Contains("get_um_cliente", body);
    }

    [Fact(DisplayName = "GET /api/cliente sem dados retorna 204")]
    [Trait("Controller", "Clientes")]
    public async Task Get_SemDados_DeveRetornar204()
    {
        // Arrange
        _factory.ClienteUseCaseMock
            .Setup(x => x.ObterTodosClientes(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(OperationResult<PageResponseDto<GetClienteDto>>.Failure("Não foi encontrado clientes", 204));

        using var client = CriarClienteAutenticado("user");

        // Act
        var response = await client.GetAsync("/api/cliente");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/cliente/{id} retorna 200 com links HATEOAS")]
    [Trait("Controller", "Clientes")]
    public async Task GetPorId_DeveRetornar200ComLinks()
    {
        // Arrange
        var cliente = new GetClienteDto
        {
            Id = 1,
            Nome = "João",
            Links = new List<LinkDto>
            {
                new LinkDto("/api/cliente/1", "get_um_cliente", "GET"),
                new LinkDto("/api/cliente/1", "update_cliente", "PUT"),
                new LinkDto("/api/cliente/1", "delete_cliente", "DELETE"),
            }
        };

        _factory.ClienteUseCaseMock
            .Setup(x => x.ObterUmCliente(1))
            .Returns(OperationResult<GetClienteDto?>.Success(cliente, 200));

        using var client = CriarClienteAutenticado("user");

        // Act
        var response = await client.GetAsync("/api/cliente/1");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"links\"", body);
        Assert.Contains("delete_cliente", body);
    }

    [Fact(DisplayName = "GET /api/cliente/{id} inexistente retorna 404")]
    [Trait("Controller", "Clientes")]
    public async Task GetPorId_Inexistente_DeveRetornar404()
    {
        // Arrange
        _factory.ClienteUseCaseMock
            .Setup(x => x.ObterUmCliente(99))
            .Returns(OperationResult<GetClienteDto?>.Failure("Não foi encontrado o cliente", 404));

        using var client = CriarClienteAutenticado("user");

        // Act
        var response = await client.GetAsync("/api/cliente/99");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "DELETE /api/cliente/{id} com role user retorna 403")]
    [Trait("Controller", "Autorizacao")]
    public async Task Delete_ComRoleUser_DeveRetornar403()
    {
        using var client = CriarClienteAutenticado("user");

        var response = await client.DeleteAsync("/api/cliente/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "DELETE /api/cliente/{id} com role admin retorna 200")]
    [Trait("Controller", "Autorizacao")]
    public async Task Delete_ComRoleAdmin_DeveRetornar200()
    {
        // Arrange
        _factory.ClienteUseCaseMock
            .Setup(x => x.DeletarCliente(1))
            .Returns(OperationResult<ClienteEntity?>.Success(new ClienteEntity { Id = 1 }, 200));

        using var client = CriarClienteAutenticado("admin");

        // Act
        var response = await client.DeleteAsync("/api/cliente/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/cliente/endereco/{cep} retorna 200 (serviço externo)")]
    [Trait("Controller", "Servico Externo")]
    public async Task GetEndereco_DeveRetornar200()
    {
        // Arrange
        _factory.ClienteUseCaseMock
            .Setup(x => x.ObterEnderecoPorCepAsync("01001000"))
            .ReturnsAsync(OperationResult<EnderecoModel?>.Success(
                new EnderecoModel { Cep = "01001-000", Logradouro = "Praça da Sé", Bairro = "Sé", Uf = "SP" }, 200));

        using var client = CriarClienteAutenticado("user");

        // Act
        var response = await client.GetAsync("/api/cliente/endereco/01001000");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Praça da Sé", body);
    }

    [Fact(DisplayName = "GET /api/cliente/endereco/{cep} inexistente retorna 404")]
    [Trait("Controller", "Servico Externo")]
    public async Task GetEndereco_Inexistente_DeveRetornar404()
    {
        // Arrange
        _factory.ClienteUseCaseMock
            .Setup(x => x.ObterEnderecoPorCepAsync("00000000"))
            .ReturnsAsync(OperationResult<EnderecoModel?>.Failure("Não foi localizado o endereco para esta cep: 00000000", 404));

        using var client = CriarClienteAutenticado("user");

        // Act
        var response = await client.GetAsync("/api/cliente/endereco/00000000");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "POST /api/auth com credenciais válidas retorna 200 e JWT")]
    [Trait("Controller", "Autenticacao")]
    public async Task PostAuth_CredenciaisValidas_DeveRetornar200()
    {
        // Arrange
        _factory.UserUseCaseMock
            .Setup(x => x.AutenticarUserAsync(It.IsAny<PostAuthDto>()))
            .ReturnsAsync(OperationResult<GetAuthDto?>.Success(new GetAuthDto { Email = "admin@fiap.com.br", Jwt = "token" }, 200));

        using var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth", new PostAuthDto { Email = "admin@fiap.com.br", Senha = "123" });
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("jwt", body);
    }

    [Fact(DisplayName = "POST /api/auth com credenciais inválidas retorna 404")]
    [Trait("Controller", "Autenticacao")]
    public async Task PostAuth_CredenciaisInvalidas_DeveRetornar404()
    {
        // Arrange
        _factory.UserUseCaseMock
            .Setup(x => x.AutenticarUserAsync(It.IsAny<PostAuthDto>()))
            .ReturnsAsync(OperationResult<GetAuthDto?>.Failure("Não foi encontrado o usuario para estas credenciais", 404));

        using var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth", new PostAuthDto { Email = "x@x.com", Senha = "errada" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
