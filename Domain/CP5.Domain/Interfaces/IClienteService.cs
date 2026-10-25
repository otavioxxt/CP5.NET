using CP5.Domain.Entities;

namespace CP5.Domain.Interfaces;

public interface IClienteService
{
    Task<EnderecoModel?> ObterEnderecoPorCepAsync(string cep);
}
