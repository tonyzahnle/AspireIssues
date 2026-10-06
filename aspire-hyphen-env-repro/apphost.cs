#:sdk Aspire.AppHost.Sdk@13.6.0
#:package Aspire.Hosting.Azure.AppContainers@13.6.0
#:package Aspire.Hosting.Azure.KeyVault@13.6.0

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("env");

// Hyphenated name -> emits both ConnectionStrings__key-vault and ConnectionStrings__key_vault on 13.6.0
var hyphenated = builder.AddAzureKeyVault("key-vault");

// Control: no hyphen -> emits only ConnectionStrings__vault
var plain = builder.AddAzureKeyVault("vault");

builder.AddContainer("app", "nginx")
	.WithReference(hyphenated)
	.WithReference(plain);

builder.Build().Run();
