# CP5 - Advanced Business Development with .NET - 2026

API RESTful em **ASP.NET Core (.NET 8) com Controllers** que gerencia **Clientes**, com seus **Produtos**, **Cursos** e **Estado**, aplicando Clean Architecture, Operation Result, Erros Customizados, consumo de serviço externo (ViaCEP), PageResult, HATEOAS e autenticação/autorização com JWT.

## Vídeo de demonstração
(https://www.youtube.com/watch?v=h_8JFNtVq_s)

## 👥 Integrantes

| Integrante | RM |
|:-----------|:--:|
| Rodrigo Silva | 565162 |
| Nickolas Davi | 564105 |
| Samara Vilela | 566133 |
| Natália Cristina | 564099 |
| Otávio Ferreira | 565960 |

**Turma:** 2TDSR · **Curso:** Análise e Desenvolvimento de Sistemas · **FIAP**
CP5.sln
├── Presentation/CP5.API              -> Controllers, Program.cs, Doc/Samples (Swagger)
├── Application/CP5.Application       -> Dtos, Interfaces, Mappers, UseCases
├── Domain/CP5.Domain                 -> Entities, Errors, Interfaces (contratos)
├── Infrastructure/CP5.Infrastructure -> Data (AppData, Repositories), Service (ViaCEP), IoC (Bootstrap)
└── Tests/CP5.Tests                   -> xUnit (unidade + funcionais)
```

Dependências: `API -> Application, Infrastructure` · `Infrastructure -> Application, Domain` · `Application -> Domain`.

| Requisito | Onde está |
|---|---|
| Clean Architecture | 4 projetos (Domain, Application, Infrastructure, API) |
| Otimização | Índices em `ClienteEntity` (composto + único), paginação com `Skip/Take`, compressão Brotli/Gzip, Rate Limit |
| Operation Result | `Domain/Entities/OperationResult.cs`, usado em todos os UseCases e lido pelas Controllers |
| Erros Customizados | `Domain/Errors/NoContentException.cs`, lançada nos Repositories e capturada nos UseCases (404/204) |
| Serviço Externo | `Infrastructure/Service/ClienteService.cs` (ViaCEP via RestSharp) -> `GET /api/cliente/endereco/{cep}` |
| PageResult | `Application/Dtos/PageResponseDto<T>` (Items, Deslocamento, RegistroRetornado, TotalRegistros) |
| HATEOAS | `LinkDto` por item e por página (`GetClienteDto`, `PageResponseDto`) |
| Autenticação/Autorização | JWT (`/api/auth`), `[Authorize]` na `ClienteController`, `[Authorize(Roles = "admin")]` no DELETE |
| Logging / Health Check | Serilog (console + arquivo) · `/health/live`, `/health/db`, `/api/health2/live`, `/api/health2/db` |
| Swagger | Annotations, exemplos (`Doc/Samples`) e botão Authorize (Bearer) |

## Configurações necessárias

`Presentation/CP5.API/appsettings.Development.json`:

- `ConnectionStrings:Oracle` -> preencha `User Id` e `Password` do Oracle (ex.: `oracle.fiap.com.br`).
- `Secretkey` -> chave usada para assinar o JWT.

Serviço externo: **ViaCEP** (`https://viacep.com.br/ws/`), sem chave.

## Como rodar

Pré-requisitos: .NET 8 SDK e acesso ao Oracle configurado acima.

```bash
dotnet restore
dotnet tool install --global dotnet-ef   # se ainda não tiver

# Cria as tabelas no Oracle (depois confira no DBeaver)
dotnet ef migrations add InitialCreate -p Infrastructure/CP5.Infrastructure -s Presentation/CP5.API
dotnet ef database update -p Infrastructure/CP5.Infrastructure -s Presentation/CP5.API

dotnet run --project Presentation/CP5.API
```

Swagger: `https://localhost:<porta>/swagger`

### Fluxo de uso
1. `POST /api/user` -> cria usuário (`role`: `admin` ou `user`; `password` = `confirmPassword`).
2. `POST /api/auth` com `email` e `senha` -> retorna o `jwt`.
3. No Swagger, clique em **Authorize** e informe o token.
4. Sem token -> **401**; token com role `user` no `DELETE /api/cliente/{id}` -> **403**; role `admin` -> **200**.

### Principais endpoints
| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/user` | Cadastra usuário (público) |
| POST | `/api/auth` | Gera o JWT (público) |
| GET | `/api/cliente?Deslocamento=0&RegistroRetornado=3` | Lista paginada com HATEOAS (rate limit 5 req/10s) |
| GET | `/api/cliente/{id}` | Cliente com links HATEOAS |
| GET | `/api/cliente/maior/idade` | Clientes maiores de idade |
| GET | `/api/cliente/endereco/{cep}` | Endereço via ViaCEP |
| POST / PUT | `/api/cliente` · `/api/cliente/{id}` | Adicionar / editar cliente |
| DELETE | `/api/cliente/{id}` | Remove cliente (apenas `admin`) |
| POST / PUT | `/api/cliente/produto/cliente/{clienteId}` · `/api/cliente/produto/{id}/cliente/{clienteId}` | Produtos do cliente |
| POST / PUT | `/api/cliente/curso/cliente/{clienteId}` · `/api/cliente/curso/{cursoId}/cliente` | Cursos do cliente |

## Como executar os testes

```bash
dotnet test
```

- **Unidade:** `ClienteRepositoryTest` (no Oracle da aplicação, removendo os dados que cria), `ClienteUseCaseTest` e `UserUseCaseTest` (Moq), `EntityTest` (Domain e Mappers).
- **Funcionais:** `ClienteControllerTest` (`WebApplicationFactory`, 401/403/200, PageResult, HATEOAS, serviço externo).

Os testes usam a mesma connection string do `appsettings.Development.json` (Oracle).
