using CP5.Application.Dtos;
using CP5.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CP5.API.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserUseCase _userUseCase;

    public UserController(IUserUseCase userUseCase)
    {
        _userUseCase = userUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Post(PostUserDto model)
    {
        var resultado = await _userUseCase.AdicionarUserAsync(model);

        if (!resultado.IsSuccess)
            return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
