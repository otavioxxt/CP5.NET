using CP5.Application.Dtos;
using CP5.Domain.Entities;

namespace CP5.Application.Interfaces;

public interface IClienteUseCase
{
    OperationResult<PageResponseDto<GetClienteDto>> ObterTodosClientes(int Deslocamento, int RegistroRetornado);
    OperationResult<GetClienteDto?> ObterUmCliente(int Id);
    OperationResult<IEnumerable<ClienteEntity>> ObterTodosClientesMaioresDeIdadeClientes(int idade);
    OperationResult<ClienteEntity?> AdicionarCliente(PostClienteDto entity);
    OperationResult<ClienteEntity?> EditarCliente(int Id, PostClienteDto entity);
    OperationResult<ClienteEntity?> DeletarCliente(int Id);
    OperationResult<ProdutoEntity?> AdicionarProdutoNoCliente(PostProdutoDto entity, int clienteId);
    OperationResult<ProdutoEntity?> EditarProdutoNoCliente(PostProdutoDto entity, int id, int clienteId);
    OperationResult<CursoEntity?> AdicionarCursoNoCliente(PostCursoDto entity, int clienteId);
    OperationResult<CursoEntity?> EditarCursoNoCliente(PostCursoDto entity, int cursoId);
    Task<OperationResult<EnderecoModel?>> ObterEnderecoPorCepAsync(string cep);
}
