var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("seq", "datalust/seq:latest")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SEQ_PASSWORD", "admin123")
    .WithHttpEndpoint(32772, targetPort:80, name: "api")
    .WithEndpoint(32771, targetPort: 5341, name: "ingestion")
    .WithVolume("seq-volume", "/data");


 builder.AddContainer("postgres", "pgvector/pgvector:pg17")
    .WithEnvironment("POSTGRES_PASSWORD", "admin")
    .WithEndpoint(32774, targetPort:5432)
    .WithVolume("postgres-volume", "/var/lib/postgresql/data");


builder.AddProject<Projects.AIQueryTool>("ai-toolbox");

builder.Build().Run();