using CP5.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace CP5.API.Doc.Samples;

public class ClienteResponseSample : IExamplesProvider<GetClienteDto>
{
    public GetClienteDto GetExamples()
    {
        return new GetClienteDto
        {
            Id = 1,
            Nome = "Marcos Oliveira",
            Email = "marcos.oliveira@outlook.com",
            Idade = 30,
            Estado = "SP",
            Links = new List<LinkDto>
            {
                new LinkDto("/api/cliente/1", "get_um_cliente", "GET"),
                new LinkDto("/api/cliente/1", "update_cliente", "PUT"),
                new LinkDto("/api/cliente/1", "delete_cliente", "DELETE"),
            }
        };
    }
}
