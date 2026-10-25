using CP5.Application.Dtos;
using CP5.Domain.Entities;

namespace CP5.Application.Mappers;

public static class UserMapper
{
    public static UserEntity ToUserEntity(this PostUserDto obj)
    {
        return new UserEntity
        {
            Nome = obj.Nome,
            Email = obj.Email,
            Password = obj.Password,
            Role = obj.Role,
        };
    }
}
