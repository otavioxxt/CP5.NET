using CP5.API.Doc.Samples;
using CP5.Application.Dtos;
using CP5.Application.Interfaces;
using CP5.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace CP5.API.Controllers;

[Authorize] // Protege todos os endpoints da Controller
[Route("api/cliente")]
[ApiController]
public class ClienteController : ControllerBase
{
    private readonly IClienteUseCase _clienteUseCase;

    public ClienteController(IClienteUseCase clienteUseCase)
    {
        _clienteUseCase = clienteUseCase;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar todos os clientes",
        Description = """
        Retorna a lista paginada de clientes cadastrados na base de dados Oracle.

        ### 📋 Informações do Retorno:
        * **Status 200 (OK):** Retorna uma página contendo os clientes localizados e os links HATEOAS.
        * **Status 204 (No Content):** Executado com sucesso, porém a base não possui registros cadastrados.
        * **Status 400 (Bad Request):** Ocorreu uma falha durante a consulta (ex: erro de conexão com o banco).

        ### 💡 Observações:
        * Os dados incluem as entidades relacionadas (como **Estado**).
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Dados do cliente listado com sucesso", type: typeof(PageResponseDto<GetClienteDto>))]
    [SwaggerResponse(statusCode: 204, description: "Nao possui dados do cliente")]
    [SwaggerResponse(statusCode: 400, description: "Ocorreu um erro ao listar os clientes", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(ClienteResponseListSample))]
    [EnableRateLimiting("politica_5_tentativas")]
    public IActionResult Get(int Deslocamento, int RegistroRetornado)
    {
        var resultado = _clienteUseCase.ObterTodosClientes(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter um cliente")]
    [SwaggerResponse(statusCode: 200, description: "Dados do cliente retornado com sucesso", type: typeof(GetClienteDto))]
    [SwaggerResponse(statusCode: 404, description: "Nao encontrado dados para o cliente")]
    [SwaggerResponseExample(statusCode: 200, typeof(ClienteResponseSample))]
    public IActionResult Get(int id)
    {
        var resultado = _clienteUseCase.ObterUmCliente(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("maior/idade")]
    [SwaggerOperation(Summary = "Lista todos os clientes maiores de idade")]
    public IActionResult GetListaMaiorDeIdade()
    {
        var resultado = _clienteUseCase.ObterTodosClientesMaioresDeIdadeClientes(18);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("endereco/{cep}")]
    [SwaggerOperation(Summary = "Obter um endereco por cep")]
    [SwaggerResponse(statusCode: 200, description: "Dados do endeco retornado com sucesso", type: typeof(EnderecoModel))]
    [SwaggerResponse(statusCode: 404, description: "Nao encontrado dados para o cep informado")]
    public async Task<IActionResult> GetEnderecoPorCep(string cep)
    {
        var resultado = await _clienteUseCase.ObterEnderecoPorCepAsync(cep);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar cliente")]
    [SwaggerRequestExample(typeof(PostClienteDto), typeof(ClienteRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Dados do cliente cadastrado com sucesso", type: typeof(ClienteEntity))]
    public IActionResult Post(PostClienteDto model)
    {
        var resultado = _clienteUseCase.AdicionarCliente(model);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Editar cliente",
        Description = """
        Atualiza os dados cadastrais completos de um cliente existente.

        ### ⚠️ Observações Importantes

        * **Email:** Deve ser único no sistema.
        * **Estado:** Informe apenas a sigla (ex: `SP`, `RJ`).

        ### 🔄 Fluxo de Processamento

        1. Valida se o `ID` existe na base.
        2. Atualiza os dados cadastrais.
        3. Retorna o objeto atualizado.
        """
    )]
    [SwaggerRequestExample(typeof(PostClienteDto), typeof(ClienteRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Dados do cliente alterados com sucesso", type: typeof(ClienteEntity))]
    public IActionResult Put([FromRoute, SwaggerParameter("Id do cliente!")] int id, PostClienteDto model)
    {
        var resultado = _clienteUseCase.EditarCliente(id, model);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [Authorize(Roles = "admin")] // Apenas usuários com a role "admin"
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Deletar cliente")]
    [SwaggerResponse(statusCode: 401, description: "Nao autenticado")]
    [SwaggerResponse(statusCode: 403, description: "Autenticado, mas sem a role admin")]
    public IActionResult Delete(int id)
    {
        var resultado = _clienteUseCase.DeletarCliente(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPost("produto/cliente/{clienteId}")]
    [SwaggerOperation(Summary = "Adicionar produto")]
    public IActionResult PostProduto(int clienteId, PostProdutoDto model)
    {
        var resultado = _clienteUseCase.AdicionarProdutoNoCliente(model, clienteId);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPut("produto/{id}/cliente/{clienteId}")]
    [SwaggerOperation(Summary = "Editar produto")]
    public IActionResult PutProduto(int id, int clienteId, PostProdutoDto model)
    {
        var resultado = _clienteUseCase.EditarProdutoNoCliente(model, id, clienteId);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPost("curso/cliente/{clienteId}")]
    [SwaggerOperation(Summary = "Adicionar curso")]
    public IActionResult PostCurso(int clienteId, PostCursoDto model)
    {
        var resultado = _clienteUseCase.AdicionarCursoNoCliente(model, clienteId);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPut("curso/{cursoId}/cliente")]
    [SwaggerOperation(Summary = "Editar curso")]
    public IActionResult PutCurso(int cursoId, PostCursoDto model)
    {
        var resultado = _clienteUseCase.EditarCursoNoCliente(model, cursoId);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
