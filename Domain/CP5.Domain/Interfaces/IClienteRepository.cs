using CP5.Domain.Entities;

namespace CP5.Domain.Interfaces;

public interface IClienteRepository
{
    (IEnumerable<ClienteEntity> data, int rowsCount) ObterTodos(int Deslocamento, int RegistroRetornado);
    IEnumerable<ClienteEntity> ObterTodosMaioresDeIdade(int idade);
    ClienteEntity? ObterUm(int Id);
    ClienteEntity? Adicionar(ClienteEntity entity);
    ClienteEntity? Editar(int Id, ClienteEntity entity);
    ClienteEntity? Deletar(int Id);
    ProdutoEntity? AdicionarProduto(int clienteId, ProdutoEntity entity);
    ProdutoEntity? EditarProduto(int Id, int clienteId, ProdutoEntity entity);
    CursoEntity? AdicionarCurso(int clienteId, CursoEntity entity);
    CursoEntity? EditarCurso(int cursoId, CursoEntity entity);
}
