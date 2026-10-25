using CP5.Application.Interfaces;
using CP5.Application.UseCases;
using CP5.Domain.Interfaces;
using CP5.Infrastructure.Data.AppData;
using CP5.Infrastructure.Data.Repositories;
using CP5.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CP5.Infrastructure.IoC;

public static class Bootstrap
{
    public static void AddIoC(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseOracle(configuration.GetConnectionString("Oracle"));
        });

        // Módulo de Usuário e Autenticação
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserUseCase, UserUseCase>();

        // Módulo de Cliente
        services.AddTransient<IClienteRepository, ClienteRepository>();
        services.AddTransient<IClienteUseCase, ClienteUseCase>();

        services.AddTransient<IClienteService, ClienteService>();
    }
}
