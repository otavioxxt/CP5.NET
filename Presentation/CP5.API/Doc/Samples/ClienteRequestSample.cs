using CP5.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace CP5.API.Doc.Samples;

public class ClienteRequestSample : IExamplesProvider<PostClienteDto>
{
    public PostClienteDto GetExamples()
    {
        return new PostClienteDto
        {
            Nome = "Joao Silva",
            Email = "joao.silva@outlook.com",
            Idade = 30,
            Estado = "SP",
        };
    }
}
