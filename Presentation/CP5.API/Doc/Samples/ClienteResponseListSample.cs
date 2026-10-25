using CP5.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace CP5.API.Doc.Samples;

public class ClienteResponseListSample : IExamplesProvider<PageResponseDto<GetClienteDto>>
{
    public PageResponseDto<GetClienteDto> GetExamples()
    {
        return new PageResponseDto<GetClienteDto>
        {
            Items = new List<GetClienteDto>
            {
                new GetClienteDto
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
                },
                new GetClienteDto
                {
                    Id = 2,
                    Nome = "Alex Ferreira",
                    Email = "alex.ferreira@outlook.com",
                    Idade = 25,
                    Estado = "RJ",
                    Links = new List<LinkDto>
                    {
                        new LinkDto("/api/cliente/2", "get_um_cliente", "GET"),
                        new LinkDto("/api/cliente/2", "update_cliente", "PUT"),
                        new LinkDto("/api/cliente/2", "delete_cliente", "DELETE"),
                    }
                }
            },
            Deslocamento = 0,
            RegistroRetornado = 2,
            TotalRegistros = 2,
            Links = new List<LinkDto>
            {
                new LinkDto("/api/cliente?Deslocamento=0&RegistroRetornado=2", "self", "GET"),
                new LinkDto("/api/cliente", "create_cliente", "POST")
            }
        };
    }
}
