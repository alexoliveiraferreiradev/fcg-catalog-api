# Fiap Cloud Games (FCG) - Catálogo API

Este microsserviço é o **núcleo de domínio do Catálogo de Jogos e Gestão de Biblioteca de Usuários** da plataforma **Fiap Cloud Games (FCG)**. Desenvolvido com **.NET 9** e baseado nos princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **CQRS**, o projeto gerencia a criação, catalogação, desativação e promoções de jogos, além de controlar quais jogos pertencem à biblioteca individual de cada usuário.

Além da gestão do catálogo, este serviço orquestra a etapa inicial das compras de jogos. Ele gera os pedidos e interage de forma assíncrona e resiliente com os demais serviços da plataforma.

---

## 🔄 Fluxos de Integração e Compras

A **Catálogo API** é responsável por iniciar e validar o ciclo de compra de jogos:

1. **Início do Pedido e Processamento**:
   - Ao realizar um pedido de jogo, a aplicação salva a intenção de compra e **produz o evento `OrderPlacedEvent`**.
   - Esse evento é consumido pelo microsserviço de *Payments*, que simulará o pagamento.

2. **Conclusão (Aprovação ou Falha)**:
   - A API possui **Consumers** aguardando a resposta financeira. Ao consumir um **`PaymentProcessedEvent`** (pagamentos aprovados), ela libera o jogo adicionando-o definitivamente à biblioteca do usuário.
   - Caso consuma um **`PaymentFailedEvent`** (pagamentos rejeitados), o status do pedido é alterado para cancelado (`Cancelled`).

3. **Garantia de Entrega e Reembolso**:
   - Em caso de erros internos inesperados ao tentar liberar o jogo na biblioteca após um pagamento já aprovado, a aplicação **produz o evento `DeliveryFailedEvent`**.
   - Esse evento alerta o ecossistema sobre a falha, sendo consumido pelo serviço de *Payments* para realizar o reembolso imediato ao usuário, protegendo as regras de negócio do sistema.

---

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 9.0**: Plataforma de desenvolvimento principal.
- **Entity Framework Core 9.0**: ORM para persistência dos dados no **SQL Server**.
- **MediatR (v14)**: Implementação do padrão Mediator para suporte a **CQRS** (Command Query Responsibility Segregation).
- **MassTransit & RabbitMQ**: Biblioteca/Framework de mensageria para integração orientada a eventos de forma assíncrona.
- **xUnit**: Framework para execução de testes de unidade e integração.

### 📦 Shared Kernels (Pacotes Compartilhados da FCG)
Projetos criados internamente e utilizados para padronizar e compartilhar recursos base:
- **Fcg.Core.Abstractions**: Biblioteca de blocos de construção para Domain-Driven Design (DDD) em projetos .NET.
- **Fcg.Core.SharedContracts**: Contratos de eventos compartilhados para comunicação assíncrona entre os microsserviços da plataforma FCG (ex: RabbitMQ / MassTransit).
- **Fcg.Core.WebApi**: Utilitários prontos para uso em ASP.NET Core Web APIs, incluindo autenticação JWT e tratamento global de exceções.

---

## 🏗️ Arquitetura da Solução

O projeto está estruturado em camadas para separar responsabilidades de forma clara e evitar acoplamento:

```
src/
├── Fcg.Catalog.Domain         # Regras de Negócio, Entidades de Domínio e Value Objects
├── Fcg.Catalog.Application    # Casos de Uso (Commands/Queries), DTOs, Event Handlers e MediatR Handlers
├── Fcg.Catalog.Infrastructure # Acesso a Dados (EF Core), Mapeamentos e Repositórios Concretos
└── Fcg.Catalog.API            # Host da API HTTP, Middlewares e Ponto de Entrada (Program.cs)
```

### Detalhamento das Camadas

#### 1. `Fcg.Catalog.Domain` (Domínio)
Contém o núcleo das regras de negócio do catálogo:
- **Entidades de Domínio**:
  - `Jogo`: Agregado raiz responsável por controlar informações do jogo, status de ativação, preço base e a lista de promoções associadas.
  - `Promocao`: Entidade que representa promoções vinculadas a um jogo específico, com validação de datas e valores.
  - `Biblioteca`: Agregado raiz que mapeia a associação de posse entre um usuário (`UsuarioId`) e um jogo (`JogoId`).
- **Objetos de Valor (Value Objects)**: `Nome`, `Descricao`, `Preco` e `Periodo` (que valida as datas de início e fim de campanhas promocionais).
- **Enums**: `GeneroJogo` (Ação, Aventura, RPG, Esporte, Estratégia, etc.).
- **Contratos (Interfaces)**: Interfaces `IJogoRepository` e `IBibliotecaRepository` para desacoplamento do banco de dados.

#### 2. `Fcg.Catalog.Application` (Aplicação)
Orquestra os fluxos de dados e implementa os casos de uso usando **CQRS**:
- **Commands & Queries**: Separados por contexto (`Biblioteca` e `Jogos`), representados como **C# records** para garantir imutabilidade.
- **Handlers**: Processam as operações de gravação e leitura via `IRequestHandler` do MediatR.
- **Event Handlers**: Estruturas de eventos preparadas para reagir a gatilhos externos (ex.: `PaymentProcessedEvent`).
- **DTOs**: Estruturas de resposta para transferir dados de forma otimizada (ex.: `JogosResponse`, `PromocaoResponse`, `BibliotecaItemResponse`).

#### 3. `Fcg.Catalog.Infrastructure` (Infraestrutura)
Lida com a comunicação técnica de persistência dos dados:
- **Persistência (EF Core)**: Configuração do `ApplicationDbContext` que carrega mapeamentos explícitos como `JogoMapping`, `PromocaoMapping` e `BibliotecaMapping`.
- **Repositórios Concretos**: Implementações das interfaces em `JogoRepository` (que gerencia o catálogo e aplica atualizações em lote de promoções expiradas) e `BibliotecaRepository`.

#### 4. `Fcg.Catalog.API` (Apresentação)
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

## ⚙️ Configuração e Variáveis de Ambiente

Para o funcionamento local stand-alone, configure o arquivo `appsettings.json`

```json
{
  "DatabaseSettings": {
    "Host": "localhost",
    "Port": "1433",
    "DatabaseName": "Fcg_Catalogs",
    "Username": "sa",
    "Password": "SuaSenhaSegura"
  },
  "RabbitMqSettings": {
    "Host": "localhost",
    "Port": "5672",
    "Username": "guest",
    "Password": "guest",
    "CatalogPaymentFailedQueue": "catalog-payment-failed-queue",
    "CatalogPaymentProcessedQueue": "catalog-payment-processed-queue"
  },
  "JwtSettings": {
    "Secret": "SUA_CHAVE_SUPER_SECRETA_E_LONGA_DE_EXEMPLO",
    "ExpirationHours": 2,
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGamesAudience"
  },
  "RedisSettings": {
    "Host": "localhost",
    "Port": "6379",
    "Password": "SuaSenhaSegura",
    "InstanceName": "FiapCloudGames:"
  }
}
```

---

## 🚀 Como Executar Localmente (Standalone)

### Pré-requisitos
- SDK do [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- SQL Server, Redis e RabbitMQ acessíveis.

### Comandos de Terminal

1. **Restaurar Dependências e Compilar:**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Aplicar Migrações do Banco de Dados (EF Core):**
   Certifique-se de que a Connection String do SQL Server esteja acessível no seu `appsettings.json` local e execute:
   ```bash
   dotnet ef database update --project src/Fcg.Catalog.Infrastructure/ --startup-project src/Fcg.Catalog.API/
   ```

3. **Executar a API:**
   ```bash
   dotnet run --project src/Fcg.Catalog.API/
   ```
   A API estará acessível e a documentação Swagger poderá ser acessada em: `http://localhost:8084/swagger`.

---

## 🐳 Construção da Imagem Docker

Para validar e construir a imagem Docker do microsserviço de forma isolada, provando o funcionamento de sua otimização por múltiplos estágios (multi-stage build) otimizada para produção, execute o seguinte comando a partir da raiz deste repositório:

```bash
docker build -t fcg-catalog-api .
```

---

## ☸️ Como Implantar no Kubernetes (k8s)

Os manifestos na pasta `/k8s` estão prontos para implantação local. Para aplicar todas as configurações, serviços de rede e deployments do microsserviço de catálogo no cluster local configurado, execute:

```bash
kubectl apply -f k8s/
```

### Validação dos Recursos
Para avaliar se a implantação foi bem-sucedida, execute:
```bash
# Verificar status dos pods (deve constar como Running)
kubectl get pods

# Verificar serviços expostos e a porta do NodePort
kubectl get services
```

*Nota: Por padrão, o serviço `svc-fcg-catalog-api` é do tipo **NodePort**, expondo a API fisicamente no localhost na porta **`30001`** para fácil acesso do avaliador.*

### 🏥 Resiliência e Probes de Saúde (Health Checks)

Como o Catálogo de Jogos depende diretamente de outras peças de infraestrutura (Banco de Dados SQL Server, cache Redis e o message broker RabbitMQ), a API possui endpoints nativos de checagem para evitar tráfego de rede caso alguma dessas dependências fique indisponível:
- **Liveness Probe (`/health/liveness`)**: Usada pelo Kubernetes para certificar que o contêiner da API está em execução normal.
- **Readiness Probe (`/health/readiness`)**: Usada pelo Kubernetes para validar a conectividade ativa com o SQL Server, Redis e RabbitMQ antes de permitir que o pod receba requisições. Caso ocorra alguma falha em qualquer uma das dependências externas, o tráfego de rede direcionado ao pod é interrompido temporariamente.

Essas sondas estão totalmente configuradas nos manifestos de deployment (`k8s/deployment.yaml`).

---

## 🧪 Testes Automatizados

O repositório conta com suítes de testes robustas organizadas na pasta `/tests`:
- **Fcg.Catalog.Domain.Tests:** Testes unitários focados nas regras de negócio do catálogo e da biblioteca.
- **Fcg.Catalog.Application.Tests:** Testes unitários focados na validação dos comandos, consultas e handlers com mocks.
- **Fcg.Catalog.Infrastructure.Integration:** Testes de persistência integrados com banco de dados real ou in-memory.

Para executar todos os testes da solução, rode o comando a partir do diretório raiz:
```bash
dotnet test
```

---
