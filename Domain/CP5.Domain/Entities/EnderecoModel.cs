namespace CP5.Domain.Entities;

public class EnderecoModel
{
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
}
