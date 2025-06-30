# Decisões Arquiteturais

## Plataforma e Framework

* **.NET Core 9:** Escolhido pela modernidade, alta performance, robustez pós-preview, segurança aprimorada e suporte de longo prazo. Além disso, oferece facilidade de manutenção, uma comunidade ativa e integração perfeita com ferramentas modernas e práticas DevOps, tornando-se ideal para soluções escaláveis e resilientes.
* **.NET Aspire:** Utilizado para observabilidade e orquestração, permitindo gerenciamento eficiente dos serviços distribuídos, diagnósticos detalhados e monitoramento contínuo através do OpenTelemetry, essenciais para garantir resiliência e alta disponibilidade.

## Flexibilidade via Configurações Centralizadas

Todas as configurações críticas da solução são provenientes do `appsettings.json`, incluindo textos de documentação do Swagger, segredos do JWT, parâmetros de versionamento das APIs, strings de conexão e integrações externas. Isso proporciona flexibilidade, agilidade em ajustes de ambiente e facilidade de automação de deploys, garantindo aderência às melhores práticas de DevOps e Cloud Native.

## Comunicação e Gateway

* **YARP (Yet Another Reverse Proxy):** Centraliza as APIs, facilita o balanceamento de carga e o dimensionamento horizontal automático, garantindo alta disponibilidade. Com ele, é possível facilmente adicionar servidores adicionais, prevenindo degradação de desempenho mesmo durante picos de uso.
* Comunicação via **HTTP** simplifica o desenvolvimento local, mas em produção é obrigatório o uso de **HTTPS** com certificados SSL/TLS válidos, garantindo segurança robusta contra interceptações e ataques man-in-the-middle.

## Persistência de Dados

* **PostgreSQL:** Banco relacional robusto, escalável e gratuito, com recursos avançados como JSONB, particionamento, suporte eficiente a transações e consultas analíticas complexas, ideal para sistemas empresariais que exigem desempenho consistente e disponibilidade.
* **Redis:** Utilizado como cache distribuído NoSQL para maximizar desempenho, diminuir latência e garantir alta disponibilidade, especialmente crucial para serviços críticos como o de consolidação diária que precisam suportar alta carga.

## Chaves Primárias

* **GUIDs:** Adotados para garantir escalabilidade horizontal e evitar gargalos em ambientes distribuídos, além de fornecer unicidade global, reduzindo problemas comuns em sistemas distribuídos como colisões de chaves. Reconheço os desafios dessa escolha, como degradação de índices, maior consumo de espaço em banco e menor amigabilidade, mas justifico-a pela versatilidade essencial em ambientes distribuídos complexos.

## Acesso a Dados

* **Entity Framework Core:** Escolhido pela simplicidade e produtividade elevada com consultas via LINQ, automação eficiente de migrações e redução significativa de boilerplate, permitindo um desenvolvimento mais ágil e manutenção mais eficiente.

## Segurança

* **Autenticação JWT Bearer:** Segurança robusta com validação rigorosa de secret, issuer e audience, essencial para proteção contra acessos não autorizados e ataques maliciosos.
* **OAuth2 com JWT:** Tokens de acesso com validade curta (15 minutos) e Refresh Tokens com validade de 14 dias, garantindo segurança aprimorada com conveniência operacional, permitindo controle rigoroso sobre as sessões dos usuários.
* **AspNetCore.DataProtection:** Proteção adicional para dados sensíveis, essencial para garantir que informações confidenciais estejam protegidas contra vazamentos e acessos indevidos.
* **BCrypt.Net-Next:** Utilizado para criptografia segura e armazenamento das senhas dos usuários no PostgreSQL. Este algoritmo oferece resistência robusta a ataques de força bruta e rainbow table, garantindo segurança de ponta para as credenciais dos usuários.
* **Preocupação com dados sensíveis:** Nenhum dado sensível (como e-mails, senhas ou tokens) é trafegado em querystrings, sempre optando pelo uso de body nas requests, evitando exposição em logs ou caches intermediários.

## CQRS e Mediação

* **CQRS com MediatR:** Implementação robusta que permite fácil manutenção, segregação clara entre comandos e consultas, melhor desempenho e escalabilidade, especialmente importante em sistemas que precisam de resiliência e disponibilidade contínua.

## Mensageria

* A mensageria, essencial para desacoplamento de serviços, comunicação assíncrona e resiliência a falhas, além de permitir escalabilidade horizontal eficiente, será implementada futuramente com preferência pelo **Kafka** devido à sua alta escalabilidade, robustez, desempenho elevado e capacidade comprovada em ambientes empresariais complexos.

## Validação e Mapeamento

* **FluentValidation:** Utilizado para garantir validações claras, consistentes e fáceis de manter, fundamental para evitar inconsistências nos dados que possam prejudicar a integridade do sistema.
* **AutoMapper:** Simplifica o mapeamento entre entidades e DTOs, aumentando produtividade, clareza e eficiência na comunicação entre camadas e microserviços.

## Observabilidade e Auditoria

* **.NET Aspire e OpenTelemetry:** Centralização eficiente da observabilidade, essencial para diagnóstico rápido de problemas, monitoramento proativo e suporte à decisão baseado em métricas. O uso adicional de ferramentas como **Prometheus e Grafana** pode ampliar ainda mais as capacidades analíticas e de monitoramento detalhado.
* **ElasticSearch/Kibana:** Centraliza todos os logs (HTTP e Application) através do OpenTelemetry, com categorização via LogType. Além disso, atua como ferramenta poderosa para auditoria e análise de logs, essencial para segurança, compliance e melhoria contínua.

## Middleware

* **Middleware de Erros Centralizado:** Padroniza respostas de erros HTTP, garantindo comunicação uniforme com o frontend e facilitando identificação rápida e correção eficaz de problemas, melhorando significativamente a experiência do desenvolvedor e do usuário final.
* **Middleware de Logs HTTP:** Captura e centraliza logs de requisições/respostas HTTP no ElasticSearch com LogType específico (HTTP), essencial para rastreabilidade e suporte eficiente na investigação de problemas.
* **Padronizador de URLs:** Middleware customizado garante que todas as rotas das APIs estejam sempre em lowercase, facilitando a leitura, padronização, documentação e evitando erros causados por diferenciação de maiúsculas/minúsculas em ambientes diversos.

## Serialização

* **Serializador/Deserializador JSON Customizado:** Desenvolvido para contornar limitações do `System.Text.Json` com CamelCase, garantindo maior flexibilidade e compatibilidade com diferentes clientes frontend, crucial para sistemas que precisam interagir com diversas plataformas e clientes.

## Versionamento de APIs

* As APIs implementam versionamento via URL e cabeçalho, permitindo múltiplas versões coexistentes e facilitando a evolução incremental dos contratos, sem quebras para os consumidores. Isso garante flexibilidade para adição ou alteração de funcionalidades sem afetar integrações existentes, além de facilitar o rollout controlado de mudanças críticas.

## Injeção de Dependência e Organização

* Todas as injeções de dependências e configurações foram agrupadas em classes de **Extensions** (ex: `DependencyInjectionExtensions`), mantendo o `Program.cs` limpo, legível e de fácil manutenção, além de permitir melhor reutilização e testabilidade das configurações.

## Autorização e RBAC

* Sistema baseado em **RBAC (Role-Based Access Control)**: Permite gerenciamento detalhado e seguro de acessos conforme funções específicas dos usuários, essencial para segurança operacional robusta e prevenção de acessos indevidos.
* Implementação de **CORS** restrito às origens "[https://localhost:7243](https://localhost:7243/)" e "[http://localhost:5199](http://localhost:5199/)", garantindo segurança adicional durante testes de carga.
* Retornos técnicos explícitos durante desenvolvimento facilitam testes e validações detalhadas, com recomendação explícita para simplificar respostas em produção por questões de segurança.

## Testes

* **XUnit, Moq e Testcontainers:** Diversos testes unitários, de integração e end-to-end foram desenvolvidos, comprovando o correto funcionamento e a robustez dos componentes da solução. O uso dessas ferramentas garante cobertura confiável, proteção contra regressões, fácil manutenção e facilita a evolução contínua da base de código.

## Containers

* **Docker com .NET Aspire:** Aplicação empacotada em containers Docker, garantindo isolamento de ambientes, facilitação em deploys contínuos e escalabilidade simplificada, essencial para ambientes modernos de desenvolvimento e operações (DevOps).

---

Essas decisões garantem uma solução escalável, segura, resiliente, eficiente e preparada para evoluções futuras, com governança clara, total controle das configurações e máxima aderência a boas práticas de engenharia de software.
