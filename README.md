# Fiap Cloud Games (FCG) - Catálogo API

Este microsserviço é o **núcleo de domínio do Catálogo de Jogos** da plataforma **Fiap Cloud Games (FCG)**. Desenvolvido com **.NET 9** e baseado nos princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **CQRS**, o projeto gerencia a criação, atualização, desativação/reativação e promoções de jogos.

A Catálogo API é a **fonte da verdade do catálogo**: ela nunca consulta outro microsserviço para responder, e é ela quem publica os eventos que os demais serviços (Order API, User Library API) usam para manter seus próprios *snapshots* locais de jogos.

---

## 🔄 Fluxo de Eventos e Responsabilidades

A comunicação com os demais microsserviços é assíncrona, via **Azure Service Bus** (tópicos e subscriptions, abstraído pelo **MassTransit** com `UsingAzureServiceBus`), com o padrão de **Outbox Transacional** (tabelas de Outbox no SQL Server) garantindo que a publicação do evento aconteça de forma consistente com a gravação no banco.

Cada evento é publicado em um **tópico com nome fixo**, definido via `SetEntityName` com as constantes da classe `Topics` do pacote `Fcg.Core.SharedContracts` (namespace `Fcg.Core.SharedContracts.Messaging`). Assim, publicadores e consumidores de todos os microsserviços enxergam os mesmos tópicos, independentemente do nome da interface do contrato.

> [!NOTE]
> A Catálogo API não depende do estado de nenhum outro microsserviço para atender requisições. O único *consumer* do serviço recebe as **avaliações dos jogadores** (veja [Eventos Consumidos](#-eventos-consumidos)), que apenas atualizam a nota média do jogo.

### 📤 Eventos Publicados

| Evento | Tópico | Publicado quando | Consumido por |
| :--- | :--- | :--- | :--- |
| `IGameCreatedIntegrationEvent` | `game-created` | Um jogo é cadastrado (`AddGameCommand`), ou via republicação manual/automática | Order API, User Library API (criam o *snapshot* local do jogo) |
| `IGameDeactiveIntegrationEvent` | `game-deactivated` | Um jogo é desativado (`DeactivateGameCommand`) | Order API, User Library API (marcam o jogo como indisponível) |
| `IGameReactiveIntegrationEvent` | `game-reactivated` | Um jogo é reativado (`ReactivateGameCommand`) | Order API, User Library API (devolvem o jogo à disponibilidade) |

### 📥 Eventos Consumidos

| Evento | Tópico | Fila (`AzureServiceBus:CatalogGameRatedIntegrationQueue`) | Consumer | Efeito |
| :--- | :--- | :--- | :--- | :--- |
| `IGameRatingIntegrationEvent` | `game-rated` | `catalog-game-rated-integration-queue` | `RatingGameConsumer` | Registra ou atualiza a nota do jogador (`RatingGameCommand`) e recalcula `AverageRating`/`RatingCount` do jogo. |

O evento é publicado pela **User Library API** quando o jogador avalia um jogo da sua biblioteca. O MassTransit cria a fila e a *subscription* que encaminha as mensagens do tópico `game-rated` para ela. O endpoint usa o **Inbox do Outbox do EF Core** (`UseEntityFrameworkOutbox`), de modo que a mesma mensagem entregue duas vezes não conta a nota duas vezes, e o handler ignora avaliações mais antigas que a já registrada (comparando `OccurredAt`). Falhas transitórias são reprocessadas 3 vezes com intervalo de 5 segundos; `DomainException` vai direto para a fila de erro, sem retry.

> [!IMPORTANT]
> **Atualizar um jogo não republica o evento de integração.** `UpdateGameCommand` só dispara o evento interno `GameUpdatedEvent` (usado para invalidar o cache Redis local) — nome, descrição, preço e gênero atualizados **não** são propagados automaticamente para os *snapshots* do Order API e do User Library API. Para ressincronizar os demais serviços após uma edição, use a republicação manual abaixo.

### 🔁 Republicação Manual de Eventos

`POST /api/admin/games/republish` (Admin) aciona `RepublishGamesEvent`, que reenvia `IGameCreatedIntegrationEvent` para **todos** os jogos cadastrados — com o preço promocional vigente, se houver. Use esse endpoint para popular ou ressincronizar os demais serviços após uma edição de jogo, uma indisponibilidade prolongada do Azure Service Bus, ou qualquer suspeita de divergência entre os *snapshots*.

Em ambiente de **Development**, essa republicação também é disparada automaticamente uma única vez, logo após o seed inicial do catálogo (`CatalogDbContextSeed`), para popular o ecossistema local com dados de teste.

### ⏱️ Desativação Automática de Promoções Expiradas

Duas rotinas garantem que promoções vencidas nunca apareçam como válidas, mesmo sem intervenção manual:
- **`DeactivateInvalidPromotionWorker`**: um `BackgroundService` que roda a cada 1 hora e desativa em lote todas as promoções cujo período de validade já expirou.
- **Invalidação sob demanda**: os principais endpoints de leitura (catálogo público, listagem/detalhe de promoções, detalhe de jogo pelo admin) disparam `DeactivatePromotionInvalidaCommand` antes de consultar o banco, garantindo consistência mesmo entre execuções do worker.

---

## 🔍 Busca de Jogos com Elasticsearch

A busca textual do catálogo (`GET /api/catalog/games/search?q=...`) é atendida pelo **Elasticsearch**, usando o client oficial `Elastic.Clients.Elasticsearch`. O SQL Server continua sendo a fonte da verdade: o índice é apenas uma cópia otimizada para busca, mantida em sincronia pela própria API.

### Índice `games`

O formato do índice fica centralizado em `GameIndex` (nome, mapping e analyzers), e cada jogo é gravado como um `GameDocument` com o `Id` do jogo como `_id`:

| Campo | Mapeamento |
| :--- | :--- |
| `name` | `text` com o analyzer `folding` (minúsculas e sem acento) + subcampo `name.stemmed` com stemmer em **inglês** (nomes de jogos costumam estar em inglês: `"knights"` → `"knight"`, `"Assassin's"` → `"assassin"`). |
| `description` | `text` com o analyzer `folding` + subcampo `description.stemmed` com stemmer leve em **português** (`"corridas"` → `"corrida"`). |
| `genre` | `text` com o analyzer `folding`. |
| `originalPrice`, `promotionPrice`, `promotionStartDate`, `promotionEndDate` | Usados para calcular o preço vigente no momento da busca. |
| `isActive` | Filtro: só jogos ativos aparecem no resultado. |
| `averageRating`, `ratingCount` | Nota média e quantidade de avaliações, devolvidas no resultado. |
| `coverUrl` | `keyword` não indexado (apenas devolvido). |

### Consulta

O `ElasticGameService` monta um `bool` com `multi_match` (`best_fields`) sobre os campos com peso `name^3`, `name.stemmed^2`, `genre^2`, `description` e `description.stemmed^0.5`, e um filtro `term` em `isActive = true`:
- **Tolerância a erro de digitação**: `fuzziness: AUTO` com `prefix_length: 1` (a primeira letra precisa bater).
- **Relevância**: quem digitou a palavra exatamente como no jogo fica acima de quem só bateu pelo radical, e o nome pesa mais que gênero e descrição.
- **Paginação**: `page`/`pageSize` (até 50 itens por página), com termo de até 100 caracteres e janela máxima de 10.000 resultados, validados por FluentValidation.

### Sincronização do índice

- **A cada mudança no jogo**: o `GameSearchIndexHandler` reage às notificações internas do MediatR (`GameAddedEvent`, `GameUpdatedEvent`, `GameDeactivatedEvent`, `GameReactivatedEvent`, `PromotionAddedEvent`, `PromotionUpdatedEvent`, `PromotionDeactivatedEvent`), lê o estado atual do jogo no banco (`GameDocumentQuery`, via Dapper) e regrava o documento — ou o remove do índice, se o jogo não existir mais. Avaliações recebidas pelo `RatingGameConsumer` disparam `GameUpdatedEvent` e, portanto, também atualizam a nota no índice. Uma falha no Elasticsearch é apenas logada: ela **não** desfaz a gravação no SQL Server.
- **Na subida da API**: o `GameSearchIndexInitializer` (`IHostedService`) verifica o índice e:
  - **não existe** → cria com o mapping atual e carrega todos os jogos do banco (`Bulk`);
  - **mapping desatualizado** (a versão gravada em `_meta.mapping_version` difere de `GameIndex.MappingVersion`, ou o índice foi criado automaticamente pelo Elasticsearch) → apaga, recria e recarrega;
  - **vazio** (carga anterior falhou) → recarrega;
  - **pronto** → não faz nada.

  Se o Elasticsearch estiver fora do ar, a API sobe mesmo assim (o erro é logado): o catálogo pelo SQL continua funcionando e apenas a busca fica indisponível.

> [!TIP]
> Ao alterar o mapping (analyzer, campo, tipo ou subcampo) em `GameIndex.Configure`, incremente `GameIndex.MappingVersion`. Na próxima subida, o índice é recriado e recarregado automaticamente — no rollout do Kubernetes, a primeira réplica corrige o índice e as seguintes o encontram pronto.

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
| `GET` | `/api/catalog/games/search?q={termo}&page=1&pageSize=10` | Busca textual de jogos ativos no **Elasticsearch** (nome, gênero e descrição), tolerante a erros de digitação e ordenada por relevância. |

A documentação **Swagger** fica disponível em `/swagger` quando a API é executada isoladamente.

### 🩺 Health Checks

| Rota | Uso |
| :--- | :--- |
| `/health/liveness` | O processo está de pé. |
| `/health/readiness` | Dependências (**SQL Server** e **Redis**) estão acessíveis. Não inclui o Azure Service Bus nem o Elasticsearch: a indisponibilidade da busca não deve tirar o catálogo do ar. |

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
- **MassTransit & Azure Service Bus**: Mensageria para publicação assíncrona dos eventos de integração em tópicos do Azure Service Bus e consumo das avaliações de jogos, com Outbox/Inbox Transacional baseado em EF Core.
- **Elasticsearch (`Elastic.Clients.Elasticsearch` 9.x)**: Motor de busca textual do catálogo, com analyzers sem acento e stemmers em inglês/português, busca *fuzzy* e ordenação por relevância. Funciona tanto com **Elastic Cloud** (`CloudId` + `ApiKey`) quanto com uma instância própria (`Url`).
- **Redis (StackExchange)**: Cache distribuído da listagem do catálogo e das promoções, invalidado automaticamente a cada mutação via handlers do MediatR.
- **Prometheus (prometheus-net.AspNetCore)**: Exposição de métricas HTTP e de negócio no formato Prometheus.
- **xUnit**: Framework para execução de testes de unidade.

### 📦 Shared Kernels (Pacotes Compartilhados da FCG)
Projetos criados internamente e utilizados para padronizar e compartilhar recursos base:
- **Fcg.Core.Abstractions**: Biblioteca de blocos de construção para Domain-Driven Design (DDD) em projetos .NET.
- **Fcg.Core.SharedContracts**: Contratos de eventos compartilhados para comunicação assíncrona entre os microsserviços da plataforma FCG via Azure Service Bus / MassTransit, incluindo a classe `Topics` com os nomes dos tópicos.
- **Fcg.Core.WebApi**: Utilitários prontos para uso em ASP.NET Core Web APIs, incluindo autenticação JWT e tratamento global de exceções.

---

## 🏗️ Arquitetura da Solução

O projeto está estruturado em camadas para separar responsabilidades de forma clara e evitar acoplamento:

```
src/
├── Fcg.Catalog.Domain         # Regras de Negócio, Entidades de Domínio e Value Objects
├── Fcg.Catalog.Application    # Casos de Uso (Commands/Queries), Event Handlers (cache e índice de busca) e MediatR Handlers
├── Fcg.Catalog.Infrastructure # Acesso a Dados (EF Core + Dapper), Outbox/MassTransit (Azure Service Bus), Busca (Elasticsearch), Cache e Worker
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
- **`GameSearchIndexHandler`**: reage às mesmas notificações internas de jogos e promoções para manter o índice do Elasticsearch igual ao banco.
- **`SearchGamesQuery`**: busca textual paginada, atendida pela abstração `IGameSearchService` (a aplicação não conhece o Elasticsearch).
- **`RatingGameCommand`**: registra a avaliação de um jogador e atualiza a nota média do jogo, a partir do evento consumido do Azure Service Bus.

#### 3. `Fcg.Catalog.Infrastructure` (Infraestrutura)
Lida com a comunicação técnica de persistência e mensageria:
- **Persistência (EF Core)**: `CatalogDbContext`, com mapeamentos explícitos (`GameMapping`, `PromotionMapping`) e as tabelas de Outbox/Inbox do MassTransit.
- **Leitura (Dapper)**: `GameQueryRepository` e `PromotionQueryRepository`, com SQL otimizado para as listagens paginadas e type handlers para os Value Objects.
- **`DeactivateInvalidPromotionWorker`**: `BackgroundService` que desativa promoções expiradas a cada hora.
- **`RedisCacheService`**: cache distribuído por trás de `ICacheService`.
- **Mensageria (`MessageBrokerExtensions`)**: MassTransit com `UsingAzureServiceBus`, mapeamento dos contratos para os tópicos (`SetEntityName(Topics.*)`), retry e o *receive endpoint* do `RatingGameConsumer`. A connection string e o nome da fila vêm de `AzureServiceBusSettings`, validados na subida (`ValidateOnStart`).
- **Busca (`Search/`)**: `ElasticGameService` (implementação de `IGameSearchService`), `GameIndex` (nome, mapping e analyzers do índice), `GameDocumentQuery` (leitura via Dapper do estado do jogo para o documento) e `GameSearchIndexInitializer` (cria/recria/carrega o índice na subida). O `ElasticsearchClient` é registrado como **singleton** (thread-safe e com pool de conexões).

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
  "AzureServiceBus": {
    "ConnectionString": "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<policy>;SharedAccessKey=<chave>",
    "CatalogGameRatedIntegrationQueue": "catalog-game-rated-integration-queue"
  },
  "RedisSettings": {
    "Host": "localhost",
    "Port": "6379",
    "Password": "SuaSenhaSegura",
    "InstanceName": "FiapCloudGames:"
  },
  "ElasticSearchSettings": {
    "Url": "http://localhost:9200",
    "ApiKey": "",
    "CloudId": ""
  },
  "JwtSettings": {
    "Secret": "SUA_CHAVE_SUPER_SECRETA_E_LONGA_DE_EXEMPLO",
    "ExpirationHours": 2,
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGamesAudience"
  }
}
```

> **AzureServiceBus** exige um namespace do Azure Service Bus no tier **Standard** ou superior (necessário para tópicos). As seções `AzureServiceBus` e `ElasticSearchSettings` são validadas na subida: sem a connection string, o nome da fila ou uma configuração válida do Elasticsearch, a API não inicia.
>
> **ElasticSearchSettings** aceita duas formas de conexão: **Elastic Cloud**, informando `CloudId` + `ApiKey`; ou **instância própria** (por exemplo, o Elasticsearch do `docker-compose` do [fcg-infrastructure](https://github.com/alexoliveiraferreiradev/fcg-infrastructure)), informando `Url` e, opcionalmente, `ApiKey`. Se `CloudId` estiver preenchido, ele tem precedência sobre `Url`.

No **Kubernetes**, essas variáveis são injetadas via `ConfigMap` (`k8s/catalog-configmap.yaml`) e segredos do **Azure Key Vault** montados pelo driver CSI (`k8s/catalog-secret-provider-class.yaml`), no formato `DatabaseSettings__Host`, `AzureServiceBus__ConnectionString`, `AzureServiceBus__CatalogGameRatedIntegrationQueue`, `ElasticSearchSettings__Url`, `ElasticSearchSettings__CloudId`, `ElasticSearchSettings__ApiKey` e assim por diante.

---

## 🚀 Como Executar Localmente (Standalone)

### Pré-requisitos
- SDK do [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- SQL Server, Redis e Elasticsearch acessíveis.
- Um namespace do **Azure Service Bus** (tier Standard ou superior) e a sua connection string.

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
