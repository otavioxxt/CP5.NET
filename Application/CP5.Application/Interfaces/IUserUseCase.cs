using CP5.Application.Dtos;
using CP5.Domain.Entities;

namespace CP5.Application.Interfaces;

public interface IUserUseCase
{
    Task<OperationResult<UserEntity?>> AdicionarUserAsync(PostUserDto entity);
    Task<OperationResult<GetAuthDto?>> AutenticarUserAsync(PostAuthDto model);
}
