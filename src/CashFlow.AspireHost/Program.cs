using CashFlow.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// ---------- Infra Containers ----------

// PostgreSQL
var postgres = builder.AddPostgres("cashflow-postgres")
    .WithImage("postgres:17-alpine")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "OpahIt2025")
    .WithEnvironment("POSTGRES_DB", "cashflowdb")
    .WithEnvironment("POSTGRES_INITDB_ARGS", "--auth-host=scram-sha-256 --auth-local=scram-sha-256")
    .WithDataVolume("cashflow-data")
    .WithEndpoint(5432, 5432, name: "postgres-port");

// Redis
var redis = builder.AddContainer("cashflow-redis", "redis:8.0.2-alpine")
    .WithArgs("redis-server")
    .WithVolume("redis_data", "/data")
    .WithEndpoint(6379, 6379, name: "redis-port");

// ---------- Aux Containers ----------

// PgAdmin
var pgadmin = builder.AddContainer("cashflow-pgadmin", "dpage/pgadmin4")
    .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@opah.com.br")
    .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "OpahIt2025")
    .WithEndpoint(5050, 80, name: "pgadmin-http");

// Redis Insight
var redisInsight = builder.AddContainer("cashflow-redisinsight", "redislabs/redisinsight:latest")
    .WithEndpoint(5540, 5540, name: "redisinsight-http");

// Elasticsearch
var elasticsearch = builder.AddContainer("cashflow-elasticsearch", "docker.elastic.co/elasticsearch/elasticsearch:9.0.0")
    .WithEnvironment("discovery.type", "single-node")
    .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m")
    .WithEnvironment("xpack.security.enabled", "false")
    .WithEnvironment("xpack.security.http.ssl.enabled", "false")
    .WithEndpoint(9200, 9200, name: "elasticsearch-http")
    .WithVolume("esdata", "/usr/share/elasticsearch/data");

// Kibana
var kibana = builder.AddContainer("cashflow-kibana", "docker.elastic.co/kibana/kibana:9.0.0")
    .WithEnvironment("ELASTICSEARCH_HOSTS", "http://cashflow-elasticsearch:9200")
    .WithEndpoint(5601, 5601, name: "kibana-http");

// Jaeger UI
var jaeger = builder.AddContainer("cashflow-jaeger", "jaegertracing/all-in-one:1.57")
    .WithEndpoint(16686, 16686, name: "jaeger-ui")
    .WithEndpoint(6831, 6831, name: "jaeger-udp")
    .WithEndpoint(6832, 6832, name: "jaeger-udp-binary")
    .WithEndpoint(14250, 14250, name: "jaeger-grpc")
    .WithEndpoint(14268, 14268, name: "jaeger-http")
    .WithEnvironment("COLLECTOR_OTLP_ENABLED", "true");

// OpenTelemetry Collector
var otelCollector = builder.AddContainer("cashflow-otel-collector", "otel/opentelemetry-collector-contrib:0.99.0")
    .WithArgs("--config=/etc/otel-collector-config.yaml")
    .WithBindMount("./otel-collector-config.yaml", "/etc/otel-collector-config.yaml")
    .WithEndpoint(4317, 4317, name: "otel-grpc")
    .WithEndpoint(4318, 4318, name: "otel-http");

// ---------- Reverse Proxy Gateway ----------

var gateway = builder.AddProject<Projects.CashFlow_ApiGateway>("cashflow-gateway")
    .WithHttpEndpoint(9000, name: "gateway");

// ---------- APIs .NET ----------

builder.AddProject<Projects.CashFlow_Entries_Api>("cashflow-entries-api")
    .WithReference(postgres)
    .WithEnvironment("Redis__ConnectionString", "{cashflow-redis.bindings.redis-port}")
    .WithEnvironment("RabbitMQ__Host", "{cashflow-rabbitmq.bindings.rabbitmq-amqp.host}")
    .WithEnvironment("RabbitMQ__Port", "{cashflow-rabbitmq.bindings.rabbitmq-amqp.port}")
    .WithEnvironment("Elasticsearch__Url", "http://{cashflow-elasticsearch.bindings.elasticsearch-http.host}:{cashflow-elasticsearch.bindings.elasticsearch-http.port}")
    .WithHttpEndpoint(8081, name: "entries");

builder.AddProject<Projects.CashFlow_Consolidations_Api>("cashflow-consolidations-api")
    .WithReference(postgres)
    .WithEnvironment("Redis__ConnectionString", "{cashflow-redis.bindings.redis-port}")
    .WithEnvironment("RabbitMQ__Host", "{cashflow-rabbitmq.bindings.rabbitmq-amqp.host}")
    .WithEnvironment("RabbitMQ__Port", "{cashflow-rabbitmq.bindings.rabbitmq-amqp.port}")
    .WithEnvironment("Elasticsearch__Url", "http://{cashflow-elasticsearch.bindings.elasticsearch-http.host}:{cashflow-elasticsearch.bindings.elasticsearch-http.port}")
    .WithHttpEndpoint(8082, name: "consolidations");

builder.AddProject<Projects.CashFlow_IdentityAndAccess_Api>("cashflow-identityandaccess-api")
    .WithReference(postgres)
    .WithEnvironment("Redis__ConnectionString", "{cashflow-redis.bindings.redis-port}")
    .WithEnvironment("RabbitMQ__Host", "{cashflow-rabbitmq.bindings.rabbitmq-amqp.host}")
    .WithEnvironment("RabbitMQ__Port", "{cashflow-rabbitmq.bindings.rabbitmq-amqp.port}")
    .WithEnvironment("Elasticsearch__Url", "http://{cashflow-elasticsearch.bindings.elasticsearch-http.host}:{cashflow-elasticsearch.bindings.elasticsearch-http.port}")
    .WithHttpEndpoint(8083, name: "identity");

// ---------- Stress test ----------

builder.AddProject<Projects.CashFlow_StressTester>("cashflow-stress-tester")
    .WithHttpEndpoint(5005, name: "stresstester");

// ---------- Setup ----------
builder.Services.AddAppHostInitializations();

builder.Build().Run();