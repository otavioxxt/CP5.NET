using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using CP5.Infrastructure.Data.AppData;
using Microsoft.EntityFrameworkCore;

namespace CP5.Infrastructure.Data.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly ApplicationContext _context;

    public ClienteRepository(ApplicationContext context)
    {
        _context = context;
    }

    public (IEnumerable<ClienteEntity> data, int rowsCount) ObterTodos(int Deslocamento = 0, int RegistroRetornado = 30)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            // Conta a quantidade total de registros na base
            var totalRegistros = _context.Cliente.Count();

            // Aplica ordenação e paginação com Skip e Take
            var resultado = _context
                .Cliente
                .Include(x => x.Estado)
                .Include(x => x.Produtos)
                .Include(x => x.Cursos)
                .OrderBy(x => x.Id)
                .Skip(Deslocamento)
                .Take(RegistroRetornado)
                .ToList();

            if (!resultado.Any())
                throw new NoContentException("Não foi encontrado clientes");

            return (resultado, totalRegistros);
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public IEnumerable<ClienteEntity> ObterTodosMaioresDeIdade(int idade)
    {
        try
        {
            var resultado = _context
                .Cliente
                .Where(x => x.Idade >= idade)
                .ToList();

            if (!resultado.Any())
                throw new NoContentException("Não foi encontrado clientes");

            return resultado;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public ClienteEntity? ObterUm(int Id)
    {
        try
        {
            var cliente = _context
                .Cliente
                .Include(x => x.Estado)
                .Include(x => x.Produtos)
                .Include(x => x.Cursos)
                .FirstOrDefault(x => x.Id == Id);

            if (cliente is null)
                throw new NoContentException("Não foi encontrado o cliente");

            return cliente;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public ClienteEntity? Adicionar(ClienteEntity entity)
    {
        try
        {
            _context.Cliente.Add(entity);
            _context.SaveChanges();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public ClienteEntity? Editar(int Id, ClienteEntity entity)
    {
        try
        {
            var cliente = _context
                .Cliente
                .Include(x => x.Estado)
                .FirstOrDefault(x => x.Id == Id);

            if (cliente is null)
                throw new NoContentException("Não foi encontrado o cliente");

            cliente.Nome = entity.Nome;
            cliente.Email = entity.Email;
            cliente.Idade = entity.Idade;

            var nomeEstado = entity.Estado?.Nome ?? string.Empty;

            if (cliente.Estado is null)
            {
                cliente.Estado = new EstadoEntity
                {
                    Nome = nomeEstado
                };
            }
            else
                cliente.Estado.Nome = nomeEstado;

            _context.Cliente.Update(cliente);
            _context.SaveChanges();

            return cliente;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public ClienteEntity? Deletar(int Id)
    {
        try
        {
            var cliente = _context.Cliente.FirstOrDefault(x => x.Id == Id);

            if (cliente is null)
                throw new NoContentException("Não foi encontrado o cliente");

            _context.Cliente.Remove(cliente);
            _context.SaveChanges();

            return cliente;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public ProdutoEntity? AdicionarProduto(int clienteId, ProdutoEntity entity)
    {
        try
        {
            var cliente = _context.Cliente.FirstOrDefault(x => x.Id == clienteId);

            if (cliente is null)
                throw new NoContentException("Não foi encontrado o cliente");

            entity.ClienteId = clienteId;

            _context.Produto.Add(entity);
            _context.SaveChanges();

            return entity;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public ProdutoEntity? EditarProduto(int Id, int clienteId, ProdutoEntity entity)
    {
        try
        {
            var produto = _context
                .Produto
                .FirstOrDefault(x => x.Id == Id && x.ClienteId == clienteId);

            if (produto is null)
                throw new NoContentException("Não foi encontrado o produto");

            produto.Nome = entity.Nome;

            _context.Produto.Update(produto);
            _context.SaveChanges();

            return produto;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public CursoEntity? AdicionarCurso(int clienteId, CursoEntity entity)
    {
        try
        {
            var cliente = _context.Cliente.FirstOrDefault(x => x.Id == clienteId);

            if (cliente is null)
                throw new NoContentException("Não foi encontrado o cliente");

            entity.ClienteId = clienteId;

            _context.Curso.Add(entity);
            _context.SaveChanges();

            return entity;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public CursoEntity? EditarCurso(int cursoId, CursoEntity entity)
    {
        try
        {
            var curso = _context.Curso.FirstOrDefault(x => x.Id == cursoId);

            if (curso is null)
                throw new NoContentException("Não foi encontrado o curso");

            curso.Nome = entity.Nome;

            _context.Curso.Update(curso);
            _context.SaveChanges();

            return curso;
        }
        catch (NoContentException ex)
        {
            throw new NoContentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}
