using CP5.Application.Dtos;
using CP5.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CP5.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserUseCase _userUseCase;

    public AuthController(IUserUseCase userUseCase)
    {
        _userUseCase = userUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Post(PostAuthDto model)
    {
        var resultado = await _userUseCase.AutenticarUserAsync(model);

        if (!resultado.IsSuccess)
            return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
