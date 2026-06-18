# Fiap Cloud Games (FCG) - Catálogo API

Este microsserviço é o **núcleo de domínio do Catálogo de Jogos e Gestão de Biblioteca de Usuários** da plataforma **Fiap Cloud Games (FCG)**. Desenvolvido com **.NET 9** e baseado nos princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **CQRS**, o projeto gerencia a criação, catalogação, desativação e promoções de jogos, além de controlar quais jogos pertencem à biblioteca individual de cada usuário.

---

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 9.0**: Plataforma de desenvolvimento principal.
- **Entity Framework Core 9.0**: ORM para persistência dos dados no **SQL Server**.
- **MediatR (v14)**: Implementação do padrão Mediator para suporte a **CQRS** (Command Query Responsibility Segregation).
- **AutoMapper (v12)**: Mapeamento de objetos entre Entidades de Domínio e DTOs.
- **xUnit**: Framework para execução de testes de unidade e integração.

---

## 🏗️ Arquitetura da Solução

O projeto está estruturado em camadas para separar responsabilidades de forma clara e evitar acoplamento:

```
src/
├── Fcg.Catalogo.Domain         # Regras de Negócio, Entidades de Domínio, Invariantes e Value Objects
├── Fcg.Catalogo.Application    # Casos de Uso (Commands/Queries), DTOs, Event Handlers e MediatR Handlers
├── Fcg.Catalogo.Infrastructure # Acesso a Dados (EF Core), Mapeamentos e Repositórios Concretos
└── Fcg.Catalogo.API            # Host da API HTTP, Middlewares e Ponto de Entrada (Program.cs)
```

### Detalhamento das Camadas

#### 1. `Fcg.Catalogo.Domain` (Domínio)
Contém o núcleo das regras de negócio do catálogo:
- **Entidades de Domínio**:
  - `Jogo`: Agregado raiz responsável por controlar informações do jogo, status de ativação, preço base e a lista de promoções associadas.
  - `Promocao`: Entidade que representa promoções vinculadas a um jogo específico, com validação de datas e valores.
  - `Biblioteca`: Agregado raiz que mapeia a associação de posse entre um usuário (`UsuarioId`) e um jogo (`JogoId`).
- **Objetos de Valor (Value Objects)**: `Nome`, `Descricao`, `Preco` e `Periodo` (que valida as datas de início e fim de campanhas promocionais).
- **Enums**: `GeneroJogo` (Ação, Aventura, RPG, Esporte, Estratégia, etc.).
- **Regras e Validações**: `AssertionConcern` para assegurar a consistência dos dados de entrada.
- **Contratos (Interfaces)**: Interfaces `IJogoRepository` e `IBibliotecaRepository` para desacoplamento do banco de dados.

#### 2. `Fcg.Catalogo.Application` (Aplicação)
Orquestra os fluxos de dados e implementa os casos de uso usando **CQRS**:
- **Commands & Queries**: Separados por contexto (`Biblioteca` e `Jogos`), representados como **C# records** para garantir imutabilidade.
- **Handlers**: Processam as operações de gravação e leitura via `IRequestHandler` do MediatR.
- **Event Handlers**: Estruturas de eventos preparadas para reagir a gatilhos externos (ex.: `PaymentProcessedEvent`).
- **DTOs**: Estruturas de resposta para transferir dados de forma otimizada (ex.: `JogosResponse`, `PromocaoResponse`, `BibliotecaItemResponse`).

#### 3. `Fcg.Catalogo.Infrastructure` (Infraestrutura)
Lida com a comunicação técnica de persistência dos dados:
- **Persistência (EF Core)**: Configuração do `ApplicationDbContext` que carrega mapeamentos explícitos como `JogoMapping`, `PromocaoMapping` e `BibliotecaMapping`.
- **Repositórios Concretos**: Implementações das interfaces em `JogoRepository` (que gerencia o catálogo e aplica atualizações em lote de promoções expiradas) e `BibliotecaRepository`.

#### 4. `Fcg.Catalogo.API` (Apresentação)
Host responsável por inicializar a API HTTP, configurar middlewares, injeção de dependências e expor endpoints de gerenciamento.

---

## 🚀 Funcionalidades Principais (Casos de Uso)

O microsserviço provê operações completas divididas nos contextos de catálogo e biblioteca:

### Gestão do Catálogo de Jogos (CRUD & Preços)
1. **Adicionar Jogo (`AdicionarJogoCommand`)**: Permite cadastrar novos jogos no sistema com nome, descrição, preço base e gênero.
2. **Atualizar Jogo (`AtualizarJogoCommand`)**: Atualiza as informações cadastrais básicas do jogo.
3. **Desativar/Reativar Jogo (`DesativarJogoCommand` / `ReativarJogoCommand`)**: Controla a visibilidade ativa do jogo no catálogo.
4. **Adicionar Promoção (`AdicionarPromocaoJogoCommand`)**: Cria campanhas de desconto para um jogo definindo um novo valor promocional e um `Periodo` de validade.
5. **Atualizar/Desativar Promoção (`AtualizarPromocaoCommand` / `DesativarPromocaoCommand`)**: Permite editar o valor e data de fim ou encerrar a promoção antecipadamente.
6. **Consultas (Queries)**:
   - Obter jogos paginados com preços atualizados (`ObtemJogosPaginadosQuery`).
   - Obter apenas jogos com promoções ativas e válidas (`ObtemJogosPromovidosPaginadosQuery`).
   - Consultar detalhes de um jogo específico pelo ID (`ObterJogoPorIdQuery`).

### Gestão de Biblioteca do Usuário
1. **Consultar Biblioteca (`ObtemBibliotecaPaginadaQuery`)**: Lista os jogos adquiridos por um usuário de forma paginada.
2. **Obter IDs Adquiridos (`ObterIdsJogosDoUsuarioQuery`)**: Recupera de forma rápida uma lista com todos os IDs de jogos pertencentes ao usuário para controle de acessibilidade ou UI.

---

## ⚙️ Configuração e Execução

### Pré-requisitos
- SDK do [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- Banco de dados SQL Server acessível.

### Configuração
No arquivo `appsettings.json` (ou `appsettings.Development.json`) do projeto `Fcg.Catalogo.API`, configure a connection string para o banco de dados:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=FcgCatalogoDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Comandos Úteis

#### Restaurar dependências e compilar a solução:
```bash
dotnet restore
dotnet build
```

#### Aplicar migrações do Entity Framework Core:
```bash
# Executar a partir da raiz do repositório
dotnet ef database update --project src/Fcg.Catalogo.Infrastructure/ --startup-project src/Fcg.Catalogo.API/
```

#### Executar a API localmente:
```bash
dotnet run --project src/Fcg.Catalogo.API/
```

---

## 🧪 Testes

O projeto conta com suítes de testes estruturadas na pasta `/tests`:

- **Fcg.Catalogo.Domain.Tests**: Testes para validação das regras de negócio do agregado de `Jogo`, `Promocao`, `Biblioteca` e Value Objects.
- **Fcg.Catalogo.Application.Tests**: Valida a lógica de execução e respostas dos MediatR Handlers.
- **Fcg.Catalogo.Infrastructure.Integration**: Testes de comunicação direta com o banco de dados e execução de queries.

Para rodar todos os testes da solução de forma unificada:
```bash
dotnet test
```

---

## 📌 Diretrizes de Contribuição e Git

Para manter o histórico do Git organizado e padronizado, todos os contribuidores devem seguir as seguintes regras:

- **Idioma dos Commits**: Todas as mensagens de commit devem ser escritas obrigatoriamente em **português**.
- **Granularidade dos Commits**: Sempre faça commits individuais e granulares por alteração lógica. Se houver `N` mudanças distintas, execute `git add` + `git commit` separadamente para cada uma delas. Nunca agrupe alterações de diferentes escopos em um único commit.
