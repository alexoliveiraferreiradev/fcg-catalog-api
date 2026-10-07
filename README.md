# Fiap Cloud Games (FCG) - Catálogo API

Este microsserviço é o **núcleo de domínio do Catálogo de Jogos** da plataforma **Fiap Cloud Games (FCG)**. Desenvolvido com **.NET 9** e baseado nos princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **CQRS**, o projeto gerencia a criação, atualização, desativação/reativação e promoções de jogos.

A Catálogo API é a **fonte da verdade do catálogo**: ela nunca consulta outro microsserviço para responder, e é ela quem publica os eventos que os demais serviços (Order API, User Library API) usam para manter seus próprios *snapshots* locais de jogos.

---

## 🔄 Fluxo de Eventos e Responsabilidades

A comunicação com os demais microsserviços é assíncrona, via **RabbitMQ** (abstraído pelo **MassTransit**), com o padrão de **Outbox Transacional** (tabelas de Outbox no SQL Server) garantindo que a publicação do evento aconteça de forma consistente com a gravação no banco.

> [!NOTE]
> Este serviço é **publicador puro**: não existe nenhum *consumer* de RabbitMQ na Catálogo API. Ela não depende do estado de nenhum outro microsserviço para atender requisições.

### 📤 Eventos Publicados

| Evento | Publicado quando | Consumido por |
| :--- | :--- | :--- |
| `IGameCreatedIntegrationEvent` | Um jogo é cadastrado (`AddGameCommand`), ou via republicação manual/automática | Order API, User Library API (criam o *snapshot* local do jogo) |
| `IGameDeactiveIntegrationEvent` | Um jogo é desativado (`DeactivateGameCommand`) | Order API, User Library API (marcam o jogo como indisponível) |
| `IGameReactiveIntegrationEvent` | Um jogo é reativado (`ReactivateGameCommand`) | Order API, User Library API (devolvem o jogo à disponibilidade) |

> [!IMPORTANT]
> **Atualizar um jogo não republica o evento de integração.** `UpdateGameCommand` só dispara o evento interno `GameUpdatedEvent` (usado para invalidar o cache Redis local) — nome, descrição, preço e gênero atualizados **não** são propagados automaticamente para os *snapshots* do Order API e do User Library API. Para ressincronizar os demais serviços após uma edição, use a republicação manual abaixo.

### 🔁 Republicação Manual de Eventos

`POST /api/admin/games/republish` (Admin) aciona `RepublishGamesEvent`, que reenvia `IGameCreatedIntegrationEvent` para **todos** os jogos cadastrados — com o preço promocional vigente, se houver. Use esse endpoint para popular ou ressincronizar os demais serviços após uma edição de jogo, uma indisponibilidade prolongada do RabbitMQ, ou qualquer suspeita de divergência entre os *snapshots*.

Em ambiente de **Development**, essa republicação também é disparada automaticamente uma única vez, logo após o seed inicial do catálogo (`CatalogDbContextSeed`), para popular o ecossistema local com dados de teste.

### ⏱️ Desativação Automática de Promoções Expiradas

Duas rotinas garantem que promoções vencidas nunca apareçam como válidas, mesmo sem intervenção manual:
- **`DeactivateInvalidPromotionWorker`**: um `BackgroundService` que roda a cada 1 hora e desativa em lote todas as promoções cujo período de validade já expirou.
- **Invalidação sob demanda**: os principais endpoints de leitura (catálogo público, listagem/detalhe de promoções, detalhe de jogo pelo admin) disparam `DeactivatePromotionInvalidaCommand` antes de consultar o banco, garantindo consistência mesmo entre execuções do worker.

---

## 🌐 Endpoints da API

### Administração (`/api/admin/...`, requer perfil **Admin**)

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/admin/games` | Lista todos os jogos (ativos e inativos). |
| `GET` | `/api/admin/games/{gameId}` | Obtém um jogo por ID. |
| `POST` | `/api/admin/games` | Cadastra um novo jogo. |
| `PUT` | `/api/admin/games/{gameId}` | Atualiza os dados de um jogo. |
| `PUT` | `/api/admin/games/{gameId}/deactivate` | Desativa um jogo. |
| `PUT` | `/api/admin/games/{gameId}/activate` | Reativa um jogo desativado. |
| `POST` | `/api/admin/games/republish` | Republica `IGameCreatedIntegrationEvent` para todos os jogos. |
| `GET` | `/api/admin/promotions` | Lista jogos com promoções cadastradas. |
| `GET` | `/api/admin/promotions/{promotionId}` | Obtém uma promoção por ID. |
| `POST` | `/api/admin/promotions` | Cria uma promoção para um jogo. |
| `PUT` | `/api/admin/promotions/{promotionId}/update` | Atualiza uma promoção existente. |
| `PUT` | `/api/admin/promotions/{promotionId}/deactivate` | Desativa uma promoção antes do prazo. |

### Catálogo Público (`/api/catalog/games`, anônimo)

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/catalog/games` | Lista paginada de jogos ativos, com filtro opcional por gênero. |
| `GET` | `/api/catalog/games/promoted` | Lista paginada de jogos ativos com promoção em vigência. |

A documentação **Swagger** fica disponível em `/swagger` quando a API é executada isoladamente.

### 🩺 Health Checks

| Rota | Uso |
| :--- | :--- |
| `/health/liveness` | O processo está de pé. |
| `/health/readiness` | Dependências (**SQL Server** e **Redis**) estão acessíveis. Não inclui o RabbitMQ, já que este serviço não consome mensagens. |

---

## 📊 Métricas (Prometheus)

A API expõe métricas no formato Prometheus em `/metrics`, habilitadas por `app.UseHttpMetrics()` (duração, contagem e status code de cada rota, coletados automaticamente) e `app.MapMetrics()`.

Além das métricas HTTP padrão, há métricas de negócio customizadas, definidas em `GameMetrics` e `PromotionMetrics` e incrementadas nos respectivos Command Handlers:

| Métrica | Tipo | Labels | Descrição |
| :--- | :--- | :--- | :--- |
| `game_create_attempts_total` | Counter | `result` | Tentativas de cadastro de jogo. |
| `game_update_attempts_total` | Counter | `result` | Tentativas de atualização de jogo. |
| `game_activate_attempts_total` | Counter | `result` | Tentativas de reativação de jogo. |
| `game_deactivate_attempts_total` | Counter | `result` | Tentativas de desativação de jogo. |
| `game_republish_attempts_total` | Counter | `result` | Tentativas de republicação em lote dos jogos. |
| `promotion_create_attempts_total` | Counter | `result` | Tentativas de criação de promoção. |
| `promotion_update_attempts_total` | Counter | `result` | Tentativas de atualização de promoção. |
| `promotion_deactivate_attempts_total` | Counter | `result` | Tentativas de desativação de promoção (manual ou automática por expiração). |

Todos os labels `result` assumem os valores `success` ou `failure`.

---

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 9.0**: Plataforma de desenvolvimento principal.
- **Entity Framework Core 9.0**: ORM utilizado na **escrita** dos dados no **SQL Server**, e também nas tabelas de Outbox do MassTransit.
- **Dapper**: Micro-ORM de alta performance utilizado na camada de **leitura** (Queries), com type handlers customizados para os Value Objects do domínio.
- **MediatR (v14)**: Implementação do padrão Mediator para suporte a **CQRS**, e canal das notificações internas que disparam a invalidação de cache.
- **MassTransit & RabbitMQ**: Mensageria para publicação assíncrona dos eventos de integração, com Outbox Transacional baseado em EF Core.
- **Redis (StackExchange)**: Cache distribuído da listagem do catálogo e das promoções, invalidado automaticamente a cada mutação via handlers do MediatR.
- **Prometheus (prometheus-net.AspNetCore)**: Exposição de métricas HTTP e de negócio no formato Prometheus.
- **xUnit**: Framework para execução de testes de unidade.

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
├── Fcg.Catalog.Application    # Casos de Uso (Commands/Queries), Event Handlers (cache) e MediatR Handlers
├── Fcg.Catalog.Infrastructure # Acesso a Dados (EF Core + Dapper), Outbox/MassTransit, Cache e Worker
└── Fcg.Catalog.API            # Host da API HTTP, Middlewares e Ponto de Entrada (Program.cs)
```

### Detalhamento das Camadas

#### 1. `Fcg.Catalog.Domain` (Domínio)
Contém o núcleo das regras de negócio do catálogo:
- **Entidades de Domínio**:
  - `Game`: Agregado raiz responsável por controlar informações do jogo, status de ativação, preço base e a lista de promoções associadas.
  - `Promotion`: Entidade que representa promoções vinculadas a um jogo específico, com validação de datas e valores.
- **Objetos de Valor (Value Objects)**: `Name`, `Description`, `Price` e `Period` (que valida as datas de início e fim de campanhas promocionais).
- **Enums**: `GameGenre` (Ação, Aventura, RPG, Esporte, Estratégia, etc.).
- **Contratos (Interfaces)**: `IGameRepository` para desacoplamento do banco de dados.

#### 2. `Fcg.Catalog.Application` (Aplicação)
Orquestra os fluxos de dados e implementa os casos de uso usando **CQRS**:
- **Commands & Queries**: Separados por contexto (Games e Promoções, cada um com uma variante Admin e uma pública), representados como **C# records** para garantir imutabilidade.
- **Handlers**: Processam as operações de gravação e leitura via `IRequestHandler` do MediatR, publicam os eventos de integração e incrementam as métricas de negócio.
- **Event Handlers de Cache**: reagem a notificações internas do MediatR (`GameAddedEvent`, `GameUpdatedEvent`, `GameDeactivatedEvent`, `GameReactivatedEvent`, `PromotionAddedEvent`, `PromotionUpdatedEvent`, `PromotionDeactivatedEvent`, `InvalidPromotionEvent`) para invalidar as chaves relevantes no Redis.
- **`RepublishGamesEvent`**: republica `IGameCreatedIntegrationEvent` para todos os jogos do catálogo.

#### 3. `Fcg.Catalog.Infrastructure` (Infraestrutura)
Lida com a comunicação técnica de persistência e mensageria:
- **Persistência (EF Core)**: `CatalogDbContext`, com mapeamentos explícitos (`GameMapping`, `PromotionMapping`) e as tabelas de Outbox/Inbox do MassTransit.
- **Leitura (Dapper)**: `GameQueryRepository` e `PromotionQueryRepository`, com SQL otimizado para as listagens paginadas e type handlers para os Value Objects.
- **`DeactivateInvalidPromotionWorker`**: `BackgroundService` que desativa promoções expiradas a cada hora.
- **`RedisCacheService`**: cache distribuído por trás de `ICacheService`.

#### 4. `Fcg.Catalog.API` (Apresentação)
Host responsável por inicializar a API HTTP, configurar middlewares, injeção de dependências e expor os endpoints, o Swagger, os health checks e as métricas.

---

## ⚙️ Configuração e Variáveis de Ambiente

Para o funcionamento local stand-alone, configure o arquivo `appsettings.json`:

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
    "Password": "guest"
  },
  "RedisSettings": {
    "Host": "localhost",
    "Port": "6379",
    "Password": "SuaSenhaSegura",
    "InstanceName": "FiapCloudGames:"
  },
  "JwtSettings": {
    "Secret": "SUA_CHAVE_SUPER_SECRETA_E_LONGA_DE_EXEMPLO",
    "ExpirationHours": 2,
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGamesAudience"
  }
}
```

No **Kubernetes**, essas variáveis são injetadas via `ConfigMap`/`Secret` (`k8s/configmap.yaml`, `k8s/secrets.yaml`), no formato `DatabaseSettings__Host`, `RabbitMqSettings__Port` e assim por diante.

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

   > Em ambiente de **Development**, a subida da API popula o catálogo com dados de exemplo (`CatalogDbContextSeed`) e, na primeira vez, republica automaticamente `IGameCreatedIntegrationEvent` para todos eles.

---

## 🐳 Construção da Imagem Docker

Para validar e construir a imagem Docker do microsserviço de forma isolada, provando o funcionamento de sua construção por múltiplos estágios (multi-stage build) otimizada para produção, execute o seguinte comando a partir da raiz deste repositório:

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

# Verificar serviços expostos
kubectl get services
```

*Nota: `svc-fcg-catalog-api` é do tipo **ClusterIP**, exposto na porta `8084` — o acesso externo se dá pelo gateway do ecossistema, não diretamente pelo serviço.*

### 🏥 Resiliência e Probes de Saúde (Health Checks)

Como o Catálogo de Jogos depende diretamente de outras peças de infraestrutura (SQL Server e Redis), a API possui endpoints nativos de checagem para evitar tráfego de rede caso alguma dessas dependências fique indisponível:
- **Liveness Probe (`/health/liveness`)**: Usada pelo Kubernetes para certificar que o contêiner da API está em execução normal.
- **Readiness Probe (`/health/readiness`)**: Usada pelo Kubernetes para validar a conectividade ativa com o SQL Server e o Redis antes de permitir que o pod receba requisições.

Essas sondas estão totalmente configuradas nos manifestos de deployment (`k8s/deployment.yaml`).

---

## 🧪 Testes Automatizados

O repositório conta com suítes de testes organizadas na pasta `/tests`:
- **Fcg.Catalog.Domain.Tests:** Testes unitários focados nas regras de negócio de `Game` e `Promotion`.
- **Fcg.Catalog.Application.Tests:** Testes unitários focados na validação dos comandos e handlers, com mocks para repositórios e publicação de eventos.

Para executar todos os testes da solução, rode o comando a partir do diretório raiz:
```bash
dotnet test
```
