using System.Net;
using CP5.Application.Dtos;
using CP5.Application.UseCases;
using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CP5.Tests.APP;

public class UserUseCaseTest
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly UserUseCase _userUseCase;

    public UserUseCaseTest()
    {
        _userRepository = new Mock<IUserRepository>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Secretkey", "0Ioc4Lr0o5K5JvbRv88G84qn0BldVdGpHshzD0bG" }
            })
            .Build();

        _userUseCase = new UserUseCase(_userRepository.Object, configuration);
    }

    [Fact]
    [Trait("UseCase", "Usuarios")]
    public async Task AutenticarUser_CredenciaisValidas_DeveRetornarJwt()
    {
        // Arrange
        var usuario = new UserEntity { Id = 1, Email = "admin@fiap.com.br", Password = "123", Role = "admin" };

        _userRepository.Setup(obj => obj.AutenticarAsync("admin@fiap.com.br", "123")).ReturnsAsync(usuario);

        // Act
        var resultado = await _userUseCase.AutenticarUserAsync(new PostAuthDto { Email = "admin@fiap.com.br", Senha = "123" });

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.OK, resultado.StatusCode);
        Assert.Equal("admin@fiap.com.br", resultado.Value!.Email);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Value.Jwt));
    }

    [Fact]
    [Trait("UseCase", "Usuarios")]
    public async Task AutenticarUser_CredenciaisInvalidas_DeveRetornar404()
    {
        // Arrange
        _userRepository
            .Setup(obj => obj.AutenticarAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new NoContentException("Não foi encontrado o usuario para estas credenciais"));

        // Act
        var resultado = await _userUseCase.AutenticarUserAsync(new PostAuthDto { Email = "x@x.com", Senha = "errada" });

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Usuarios")]
    public async Task AdicionarUser_DeveRetornarUsuarioAdicionado()
    {
        // Arrange
        _userRepository
            .Setup(obj => obj.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync((UserEntity entity) => entity);

        var dto = new PostUserDto { Nome = "Admin", Email = "admin@fiap.com.br", Password = "123", ConfirmPassword = "123", Role = "admin" };

        // Act
        var resultado = await _userUseCase.AdicionarUserAsync(dto);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal("admin", resultado.Value!.Role);
    }
}
