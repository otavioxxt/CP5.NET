using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using CP5.Infrastructure.Data.AppData;
using Microsoft.EntityFrameworkCore;

namespace CP5.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;

    public UserRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> AdicionarAsync(UserEntity entity)
    {
        try
        {
            await _context.Usuario.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<UserEntity?> AutenticarAsync(string email, string password)
    {
        try
        {
            var usuario = await _context
                .Usuario
                .FirstOrDefaultAsync(item => item.Email == email && item.Password == password);

            if (usuario is null)
                throw new NoContentException("Não foi encontrado o usuario para estas credenciais");

            return usuario;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}
