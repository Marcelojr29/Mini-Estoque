# Expedição CD: API de Pedidos para Centro de Distribuição

API REST em **.NET 10** que modela a expedição de pedidos em um Centro de Distribuição:
cadastro de produtos com controle de estoque, ciclo de vida do pedido
(criação → separação → despacho) e cálculo de frete por modalidade.

Construída com **Domain-Driven Design**, **Clean Architecture**, **SOLID** e **Entity Framework Core**.

## Como executar

Pré-requisito: .NET SDK 10.

```bash
dotnet run --project Teste.Api --launch-profile http
```

- Documentação interativa (Scalar): http://localhost:5163/scalar
- Contrato OpenAPI: http://localhost:5163/openapi/v1.json
- Roteiro de requisições: [Teste.Api/Teste.Api.http](Teste.Api/Teste.Api.http)

O banco SQLite (`teste.db`) é criado automaticamente com 5 produtos de exemplo.
Para usar **SQL Server**, basta alterar o `appsettings.json`:

```json
"ConnectionStrings": { "Default": "Server=localhost;Database=ExpedicaoCD;Trusted_Connection=True;TrustServerCertificate=True" },
"Database": { "Provider": "SqlServer" }
```

Testes:

```bash
dotnet test
```

## Arquitetura

```
            ┌──────────────┐
            │  Teste.Api   │  Controllers, tratamento global de erros, OpenAPI
            └──────┬───────┘
                   │
       ┌───────────▼───────────┐        ┌──────────────────────┐
       │   Teste.Application   │◄───────┤ Teste.Infrastructure │  EF Core, DbContext,
       │ Services, DTOs, casos │        │ Repositórios, Seed   │  mapeamentos, SQLite/SQL Server
       │ de uso                │        └──────────┬───────────┘
       └───────────┬───────────┘                   │
                   │                               │
            ┌──────▼───────────────────────────────▼┐
            │             Teste.Domain              │  Entidades, Value Objects, Guards,
            │     (não depende de NINGUÉM)          │  Strategies, contratos dos repositórios
            └───────────────────────────────────────┘
```

As dependências apontam **para dentro**: o domínio não conhece banco, HTTP nem frameworks.
A Infrastructure implementa as interfaces definidas no Domain (Inversão de Dependência).

| Projeto | Responsabilidade | Pastas |
|---|---|---|
| `Teste.Domain` | Regras de negócio | `Entities`, `ValueObjects`, `Enums`, `Guards`, `Exceptions`, `Interfaces`, `Strategies`, `Services` |
| `Teste.Application` | Orquestração dos casos de uso | `Services`, `DTOs`, `Interfaces`, `Mappings`, `Exceptions` |
| `Teste.Infrastructure` | Persistência | `Data` (DbContext, Configurations, Seed), `Repositories` |
| `Teste.Api` | Entrada HTTP | `Controllers`, `Middlewares` |
| `Teste.Tests` | Testes (xUnit) | `Domain` (unitários), `Application` (integração com SQLite em memória) |

## Modelo de domínio

- **Agregado `Produto`**: SKU, nome, peso e saldo em estoque. Regras: não reservar além do disponível.
- **Agregado `Pedido`** (raiz) com a entidade interna **`ItemPedido`**.
- **Value Objects**: `Sku`, `Endereco`, `Frete`.

Ciclo de vida do pedido:

```
Criado ──► EmSeparacao ──► Separado ──► Despachado
   │            │              │
   └────────────┴──────────────┴──► Cancelado (devolve o estoque)
```

Itens só podem ser adicionados/removidos enquanto o pedido está `Criado`.

### Frete (Strategy Pattern)

| Modalidade | Cálculo | Prazo Sul (PR/SC/RS) | Demais regiões |
|---|---|---|---|
| Econômico | R$ 15,00 + R$ 2,50/kg | 5 dias | 10 dias |
| Expresso | R$ 30,00 + R$ 4,90/kg | 1 dia | 3 dias |
| Retirada na Loja | Grátis | 2 dias | 6 dias |

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/produtos` | Lista produtos |
| GET | `/api/produtos/{id}` | Detalha produto |
| POST | `/api/produtos` | Cadastra produto |
| POST | `/api/produtos/{id}/entrada-estoque` | Entrada de mercadoria |
| GET | `/api/pedidos?status=` | Lista pedidos (filtro opcional) |
| GET | `/api/pedidos/{id}` | Detalha pedido |
| POST | `/api/pedidos` | Cria pedido |
| POST | `/api/pedidos/{id}/itens` | Adiciona item (reserva estoque) |
| DELETE | `/api/pedidos/{id}/itens/{produtoId}` | Remove item (devolve estoque) |
| POST | `/api/pedidos/{id}/iniciar-separacao` | Inicia separação |
| POST | `/api/pedidos/{id}/concluir-separacao` | Conclui separação |
| GET | `/api/pedidos/{id}/cotacoes-frete` | Cota todas as modalidades |
| POST | `/api/pedidos/{id}/despachar` | Calcula o frete e despacha |
| POST | `/api/pedidos/{id}/cancelar` | Cancela e devolve o estoque |

### Erros

Os erros são tratados em um único lugar ([ExceptionMiddleware](Teste.Api/Middlewares/ExceptionMiddleware.cs)) e retornam:

```json
{ "status": 422, "mensagem": "Estoque insuficiente para o SKU TAPETE-SALA-01. Disponível: 16, solicitado: 999." }
```

| Status | Quando |
|---|---|
| 400 | Campo obrigatório ausente ou inválido (validação dos DTOs) |
| 404 | Pedido/produto não encontrado |
| 422 | Regra de negócio violada (ex.: estoque insuficiente, transição de status inválida) |
| 500 | Erro inesperado (sem detalhes internos expostos) |

## Próximos passos

- Migrations do EF Core no lugar de `EnsureCreated`
- Autenticação/autorização com JWT
- Controle de concorrência no estoque (dois pedidos reservando o último item ao mesmo tempo)
- `CancellationToken` nos métodos assíncronos
- Paginação na listagem de pedidos
- Domain Events (ex.: `PedidoDespachado` avisando a transportadora)
- Testes de API com `WebApplicationFactory`
- Docker e health checks
