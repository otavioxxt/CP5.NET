using System.Net;
using CP5.Application.Dtos;
using CP5.Application.Interfaces;
using CP5.Application.Mappers;
using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CP5.Application.UseCases;

public class ClienteUseCase : IClienteUseCase
{
    private readonly ILogger<ClienteUseCase> _logger;
    private readonly IClienteRepository _clienteRepository;
    private readonly IClienteService _clienteService;

    public ClienteUseCase(
        IClienteRepository clienteRepository,
        ILogger<ClienteUseCase> logger,
        IClienteService clienteService)
    {
        _clienteService = clienteService;
        _clienteRepository = clienteRepository;
        _logger = logger;
    }

    private static List<LinkDto> LinksCliente(int id) => new()
    {
        new LinkDto($"/api/cliente/{id}", "get_um_cliente", "GET"),
        new LinkDto($"/api/cliente/{id}", "update_cliente", "PUT"),
        new LinkDto($"/api/cliente/{id}", "delete_cliente", "DELETE"),
    };

    public OperationResult<PageResponseDto<GetClienteDto>> ObterTodosClientes(int Deslocamento, int RegistroRetornado)
    {
        try
        {
            _logger.LogInformation("ObterTodos iniciado na application");
            var result = _clienteRepository.ObterTodos(Deslocamento, RegistroRetornado);

            var pageResponse = new PageResponseDto<GetClienteDto>
            {
                // Mapeando cada item da lista com seus próprios links de ação
                Items = result.data.Select(item => item.ToClienteDto(LinksCliente(item.Id))).ToList(),
                Deslocamento = Deslocamento,
                RegistroRetornado = RegistroRetornado,
                TotalRegistros = result.rowsCount,
                // Links globais da listagem paginada
                Links = new List<LinkDto>
                {
                    new LinkDto($"/api/cliente?Deslocamento={Deslocamento}&RegistroRetornado={RegistroRetornado}", "self", "GET"),
                    new LinkDto("/api/cliente", "create_cliente", "POST")
                }
            };

            return OperationResult<PageResponseDto<GetClienteDto>>.Success(pageResponse, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Nao retornou nenhuma informação do ObterTodos - Parametros: D: {0} - R: {1}", Deslocamento, RegistroRetornado);
            return OperationResult<PageResponseDto<GetClienteDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<PageResponseDto<GetClienteDto>>.Failure("Ocorreu um erro ao buscar todos os clientes", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<GetClienteDto?> ObterUmCliente(int Id)
    {
        try
        {
            _logger.LogInformation("ObterUmCliente iniciado na application");
            var result = _clienteRepository.ObterUm(Id);

            return OperationResult<GetClienteDto?>.Success(result?.ToClienteDto(LinksCliente(Id)), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Nao retornou nenhuma informação do ObterUmCliente - Parametros: ID: {0}", Id);
            return OperationResult<GetClienteDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetClienteDto?>.Failure("Ocorreu um erro ao obter um cliente", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<IEnumerable<ClienteEntity>> ObterTodosClientesMaioresDeIdadeClientes(int idade)
    {
        try
        {
            _logger.LogInformation("ObterTodosClientesMaioresDeIdadeClientes iniciado na application");

            var result = _clienteRepository.ObterTodosMaioresDeIdade(idade);

            return OperationResult<IEnumerable<ClienteEntity>>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<IEnumerable<ClienteEntity>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return OperationResult<IEnumerable<ClienteEntity>>.Failure("Ocorreu um erro ao buscar clientes maior de idade", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<ClienteEntity?> AdicionarCliente(PostClienteDto entity)
    {
        try
        {
            _logger.LogInformation("AdicionarCliente iniciado na application");
            var result = _clienteRepository.Adicionar(entity.ToClienteEntity());

            return OperationResult<ClienteEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<ClienteEntity?>.Failure("Ocorreu um erro ao salvar o cliente", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<ClienteEntity?> EditarCliente(int Id, PostClienteDto entity)
    {
        try
        {
            _logger.LogInformation("EditarCliente iniciado na application");
            var result = _clienteRepository.Editar(Id, entity.ToClienteEntity());

            return OperationResult<ClienteEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Nao retornou nenhuma informação do EditarCliente - Parametros: ID: {0}", Id);
            return OperationResult<ClienteEntity?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<ClienteEntity?>.Failure("Ocorreu um erro ao editar o cliente", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<ClienteEntity?> DeletarCliente(int Id)
    {
        try
        {
            _logger.LogInformation("DeletarCliente iniciado na application");
            var result = _clienteRepository.Deletar(Id);

            return OperationResult<ClienteEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Nao retornou nenhuma informação do DeletarCliente - Parametros: ID: {0}", Id);
            return OperationResult<ClienteEntity?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<ClienteEntity?>.Failure("Ocorreu um erro ao deletar o cliente", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<ProdutoEntity?> AdicionarProdutoNoCliente(PostProdutoDto entity, int clienteId)
    {
        try
        {
            _logger.LogInformation("AdicionarProdutoNoCliente iniciado na application");
            var result = _clienteRepository.AdicionarProduto(clienteId, entity.ToProdutoEntity());

            return OperationResult<ProdutoEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<ProdutoEntity?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<ProdutoEntity?>.Failure("Ocorreu um erro ao adicionar o produto", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<ProdutoEntity?> EditarProdutoNoCliente(PostProdutoDto entity, int id, int clienteId)
    {
        try
        {
            _logger.LogInformation("EditarProdutoNoCliente iniciado na application");
            var result = _clienteRepository.EditarProduto(id, clienteId, entity.ToProdutoEntity());

            return OperationResult<ProdutoEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<ProdutoEntity?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<ProdutoEntity?>.Failure("Ocorreu um erro ao editar o produto", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<CursoEntity?> AdicionarCursoNoCliente(PostCursoDto entity, int clienteId)
    {
        try
        {
            _logger.LogInformation("AdicionarCursoNoCliente iniciado na application");
            var result = _clienteRepository.AdicionarCurso(clienteId, entity.ToCursoEntity());

            return OperationResult<CursoEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<CursoEntity?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<CursoEntity?>.Failure("Ocorreu um erro ao adicionar o curso", (int)HttpStatusCode.BadRequest);
        }
    }

    public OperationResult<CursoEntity?> EditarCursoNoCliente(PostCursoDto entity, int cursoId)
    {
        try
        {
            _logger.LogInformation("EditarCursoNoCliente iniciado na application");
            var result = _clienteRepository.EditarCurso(cursoId, entity.ToCursoEntity());

            return OperationResult<CursoEntity?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<CursoEntity?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<CursoEntity?>.Failure("Ocorreu um erro ao editar o curso", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<EnderecoModel?>> ObterEnderecoPorCepAsync(string cep)
    {
        try
        {
            var result = await _clienteService.ObterEnderecoPorCepAsync(cep);

            return OperationResult<EnderecoModel?>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<EnderecoModel?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception)
        {
            return OperationResult<EnderecoModel?>.Failure("Ocorreu um erro ao buscar o cep", (int)HttpStatusCode.BadRequest);
        }
    }
}
