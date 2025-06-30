## Introdução

Este documento descreve detalhadamente a arquitetura e as tecnologias escolhidas para o desenvolvimento do projeto CashFlow, justificando cada decisão com base nas necessidades técnicas e não funcionais descritas no desafio, além das melhores práticas de mercado.

## Requisitos Funcionais

- **Controle de Lançamentos**: O sistema deve permitir o registro e controle de lançamentos diários, diferenciando débitos e créditos.

- **Relatório Consolidado Diário**: O sistema deve gerar um relatório que consolide e apresente o saldo diário das transações.

## Requisitos Não Funcionais

- **Escalabilidade**: Capacidade de lidar com aumento de carga sem degradação significativa de desempenho. Considerar dimensionamento horizontal, balanceamento de carga e estratégias de cache.

- **Resiliência**: Projeto para recuperação de falhas. Deve incluir redundância, failover, monitoramento proativo e estratégias de recuperação.

- **Segurança**: Implementação de autenticação, autorização e criptografia. Proteção contra ataques e ameaças.

- **Integração**: Definição clara da comunicação entre componentes. Avaliação de protocolos, formatos de mensagens e ferramentas de integração.

- **Disponibilidade e Confiabilidade**: Serviço de controle de lançamentos não pode ficar indisponível se o sistema de relatório diário consolidado falhar. Em dias de pico, o serviço consolidado diário deve suportar até 50 requisições por segundo, com no máximo 5% de perda de requisições.

- **Desempenho**: Otimização para garantir performance adequada do sistema.

- **Tecnologias e Boas Práticas**: Uso obrigatório de C#. Aplicação de boas práticas como Design Patterns, princípios SOLID e padrões de arquitetura.

## Estrutura Geral

O projeto foi estruturado no padrão arquitetural de microsserviços, dividido em três serviços principais:

- **CashFlow.IdentityAndAccess** (gestão de usuários e autenticação)

- **CashFlow.Entries** (gestão dos lançamentos financeiros)

- **CashFlow.Consolidations** (consulta dos dados consolidados)

Essa separação visa assegurar independência, resiliência e escalabilidade, permitindo que cada serviço opere de forma autônoma, com seu próprio ciclo de desenvolvimento, deploy e monitoramento.

## Arquitetura

O projeto segue princípios de Domain-Driven Design (DDD) e Clean Architecture. Esta escolha garante alta coesão e baixo acoplamento, permitindo o reaproveitamento de código (DRY) e uma aplicação clara dos princípios SOLID.

## Estrutura do Projeto

A solução foi organizada conforme definido no desenho a seguir, garantindo clareza, organização lógica, e facilidade de manutenção:

- Camadas claramente separadas em **Domain**, **Application**, **Infrastructure**, e **Api**.

- Serviços definidos conforme domínios específicos (Identity, Entries, Consolidations).
  
  ```
  ashFlow.sln
  Soluction Items/
  src/
  ├── CashFlow.ApiGateway
  ├── CashFlow.AppHost
  │   └── Extensions
  │   └── Setup
  ├── CashFlow.BuildingBlocks
  │   └── Domain
  │       └── Caching
  ├── CashFlow.Consolidations.Api
  │   ├── Controllers
  │   │   └── V1
  │   └── Extensions
  ├── CashFlow.Consolidations.Application
  │   ├── DailyEntries
  │   │   ├── Dto
  │   │   ├── Query
  │   │   └── QueryHandler
  │   └── Extensions
  ├── CashFlow.Consolidations.Api
  │   ├── Controllers
  │   │   └── V1
  │   └── Extensions
  ├── CashFlow.Entries.Application
  │   ├── Categories
  │   │   ├── Command
  │   │   ├── CommandHandler
  │   │   ├── Dto
  │   │   ├── Mapping
  │   │   ├── Query
  │   │   ├── QueryHandler
  │   │   ├── Validator
  │   ├── Entries
  │   │   ├── Command
  │   │   ├── CommandHandler
  │   │   ├── Dto
  │   │   ├── Mapping
  │   │   ├── Query
  │   │   ├── QueryHandler
  │   │   ├── Validator
  │   └── Extensions
  ├── CashFlow.Entries.Domain
  │   ├── Categories
  │   │   └── Repositories
  │   └── Entries
  │       ├── Entities
  │       └── Repositories
  ├── CashFlow.Entries.Infrastructure
  │   ├── Data
  │   ├── DependencyInjection
  │   ├── Extensions
  │   ├── Migrations
  │   └── Repositories
  ├── CashFlow.IdentityAndAccess.Api
  │   ├── Controllers
  │   │   └── V1
  │   └── Extensions
  ├── CashFlow.IdentityAndAccess.Application
  │   ├──  Extensions
  │   ├── Roles
  │   │   ├── Dto
  │   │   ├── Mapping
  │   │   ├── Query
  │   │   ├── QueryHandler
  │   │   ├── Validator
  │   └── Users
  │       ├── Command
  │       ├── CommandHandler
  │       ├── Dto
  │       ├── Mapping
  │       ├── Query
  │       ├── QueryHandler
  │       └── Validator
  ├── CashFlow.IdentityAndAccess.Domain
  │   ├── Entities
  │   ├── Roles
  │   │   └── Repositories
  │   ├── Services
  │   └── Users
  │       ├── Entities
  │       └── Repositories
  ├── CashFlow.Entries.Infrastructure
  │   ├── Authentication
  │   ├── Data
  │   ├── DependencyInjection
  │   ├── Extensions
  │   ├── Migrations
  │   ├── Repositories
  │   └── Security
  ├── CashFlow.Infrastructure.Shared
  │   ├── ApiVersioning
  │   ├── Authentication
  │   ├── Caching
  │   ├── Cors
  │   ├── Data
  │   ├── DependencyInjection
  │   ├── Exceptions
  │   ├── Identity
  │   ├── Logging
  │   └── Swagger
  ├── CashFlow.ServiceDefaults
  ├── CashFlow.Shared
  │   ├── Abstractions
  │   ├── Dtos
  │   ├── Exceptions
  │   │   ├── Application
  │   │   └── Application
  │   ├── Logging
  │   ├── Serialization
  │   ├── Utilities
  │   └── ValueObjects
  └── CashFlow.StressTester
      ├── Layout
      └── Pages   
  tests/
  ├── CashFlow.Consolidations.Tests
  │   └── DailyEntries
  ├── CashFlow.Entries.Tests
  │   └── Application
  │       ├── Categories
  │       │   ├── CommandHandler
  │       │   └── QueryHandler
  │       └── Categories
  │           ├── CommandHandler
  │           └── QueryHandler
  ├── CashFlow.IdentityAndAccess.Tests
  │   └── Application
  │       ├── Roles
  │       │   └── QueryHandler
  │       └── Users
  │           ├── CommandHandler
  │           └── QueryHandler
  ├── CashFlow.Shared.Tests
  │   ├── Caching
  │   ├── Exceptions
  │   └── Identity
  └── CashFlow.Shared.Tests
     └── Utilities
  Docs/
  ├── decisions.md
  └── diagrams/
  README.md
  ```

# Como Executar a Solução CashFlow

A solução **CashFlow** é totalmente orquestrada pelo **.NET Aspire**. Isso significa que você não precisa se preocupar em subir cada serviço manualmente: o projeto principal (`CashFlow.AppHost`) é responsável por iniciar e orquestrar todos os demais serviços e dependências automaticamente.

## Pré-requisitos

- **Docker** instalado e rodando
  
  > ⚠️ Recomenda-se utilizar um ambiente Docker limpo, sem containers ou imagens antigas, para evitar conflitos de porta ou cache indesejado.
  > 
  > **Recomendação**: Docker Desktop (Windows/Mac), Docker Engine (Linux) ou Rancher Desktop.
  > 
  > Certifique-se de que o Docker está rodando normalmente antes de iniciar o próximo passo.

- **.NET 8 SDK** ou superior instalado na máquina

- **Git** para clonar o repositório

- (Opcional) **Postman** para testar as APIs facilmente

## Passo a Passo

### 1. Baixe o repositório

```bash
git clone https://github.com/rkdcoder/cashflow.git
cd cashflow
```

### 2. Prepare os containers necessários

Execute o arquivo abaixo para baixar todas as imagens Docker utilizadas na solução (PostgreSQL, Redis, RabbitMQ, etc):

```powershell
.\download-docker-images.bat
```

Ou, se estiver em Linux/Mac:

```bash
sh download-docker-images.sh
```

> Isso garante que todos os containers necessários já estarão disponíveis localmente, evitando lentidão ou problemas na primeira execução.

### 3. Execute a solução

Execute o projeto **CashFlow.AppHost**. Ele é responsável por inicializar todos os microsserviços e suas dependências utilizando o **.NET Aspire**.

No terminal/prompt, rode:

```bash
dotnet run --project CashFlow.AppHost
```

Ou, se estiver usando Visual Studio, basta marcar o projeto `CashFlow.AppHost` como *startup* e pressionar F5.

### 4. Acesse o dashboard do .NET Aspire

Após a inicialização, a interface do .NET Aspire será aberta automaticamente no navegador, permitindo monitorar todos os serviços, logs, status de containers, dependências, health checks, etc.

> Todos os serviços (APIs, banco de dados, mensageria, cache, etc) serão inicializados juntos de maneira automatizada, conforme definido pela orquestração do Aspire.

---

## Testando as APIs

Dentro da pasta **Docs/** há o arquivo:

- **Teste Opah.postman_collection.json**

Esse arquivo pode ser importado no **Postman** para facilitar a execução de todos os cenários de teste e validação das APIs.  
Siga estes passos:

1. Abra o Postman

2. Importe o arquivo `Docs/Teste Opah.postman_collection.json`

3. Ajuste (se necessário) a variável de ambiente para o endpoint correto (`localhost` ou conforme indicado pelo Aspire)

---

## Teste de Estresse

O projeto **CashFlow.StressTester** é uma página Blazor que pode ser utilizada para realizar testes de carga, simulando 50 requisições por segundo na API de consolidação diária, conforme solicitado no desafio.

Para rodar o stress test:

1. Acesse a interface web do stress tester (link exibido no console)

2. Configure as opções desejadas e execute o teste

---

## Observações Finais

- Certifique-se de que as portas utilizadas pelos containers estão livres. Caso tenha conflitos, pare e remova containers antigos com `docker ps -a` e `docker rm`.

- O projeto foi desenhado para máxima simplicidade na execução, mas total controle e visibilidade para quem deseja customizar ou monitorar cada serviço separadamente.

- Consulte a documentação em **Docs/** para detalhes de arquitetura, decisões técnicas e diagramas.

---

Dúvidas ou problemas? Consulte os logs da Aspire UI e da aplicação para identificar eventuais erros na subida dos containers ou serviços.
