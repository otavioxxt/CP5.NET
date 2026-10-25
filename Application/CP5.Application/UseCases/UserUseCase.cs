using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using CP5.Application.Dtos;
using CP5.Application.Interfaces;
using CP5.Application.Mappers;
using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CP5.Application.UseCases;

public class UserUseCase : IUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserUseCase(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<OperationResult<UserEntity?>> AdicionarUserAsync(PostUserDto entity)
    {
        try
        {
            var result = await _userRepository.AdicionarAsync(entity.ToUserEntity());

            return OperationResult<UserEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (Exception)
        {
            return OperationResult<UserEntity?>.Failure("Ocorreu um erro ao adicionar o usuario", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetAuthDto?>> AutenticarUserAsync(PostAuthDto model)
    {
        try
        {
            var result = await _userRepository.AutenticarAsync(model.Email, model.Senha);

            // Autenticação e Geração do JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["Secretkey"]!.ToString());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, result!.Email.ToString()),
                    new Claim(ClaimTypes.Role, result!.Role.ToString()),
                })
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var userAutenticado = new GetAuthDto
            {
                Email = model.Email,
                Jwt = tokenHandler.WriteToken(token),
            };

            return OperationResult<GetAuthDto?>.Success(userAutenticado, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetAuthDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception)
        {
            return OperationResult<GetAuthDto?>.Failure("Ocorreu um erro ao autenticar o usuario", (int)HttpStatusCode.BadRequest);
        }
    }
}
