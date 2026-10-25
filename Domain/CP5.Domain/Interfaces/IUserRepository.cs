using CP5.Domain.Entities;

namespace CP5.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> AdicionarAsync(UserEntity entity);
    Task<UserEntity?> AutenticarAsync(string email, string password);
}
