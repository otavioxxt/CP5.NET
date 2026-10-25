using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CP5.Infrastructure.Service;

public class ClienteService : IClienteService
{
    private readonly ILogger<ClienteService> _logger;
    private readonly RestClient _restClient;

    public ClienteService(ILogger<ClienteService> logger)
    {
        _logger = logger;
        _restClient = new RestClient("https://viacep.com.br/ws/");
    }

    public async Task<EnderecoModel?> ObterEnderecoPorCepAsync(string cep)
    {
        var request = new RestRequest($"{cep}/json/", Method.Get);
        var response = await _restClient.ExecuteAsync<EnderecoModel>(request);

        // ViaCEP responde 200 com {"erro": true} quando o CEP tem formato válido mas não existe
        if (!response.IsSuccessful || response.Data == null || string.IsNullOrEmpty(response.Data.Cep))
            throw new NoContentException($"Não foi localizado o endereco para esta cep: {cep}");

        return response.Data;
    }
}
