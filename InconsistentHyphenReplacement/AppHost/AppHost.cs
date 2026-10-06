#:sdk Aspire.AppHost.Sdk@13.6.0
#:package Aspire.Hosting.Azure.KeyVault@13.6.0

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Azure.Provisioning.KeyVault;

Console.WriteLine("Hello, world!");

var builder = DistributedApplication.CreateBuilder(args);

var keyVaultResourceName = "key-vault";

IResourceBuilder<IResourceWithConnectionString> AddKeyVaultCustom(IDistributedApplicationBuilder builder)
{
    if (!builder.ExecutionContext.IsPublishMode)
    {
        var keyVault = builder.AddConnectionString(keyVaultResourceName);
        return keyVault;
    }
    else
    {
        var keyVault = builder.AddAzureKeyVault(keyVaultResourceName)
            .ConfigureInfrastructure(infra =>
            {
                var vault = infra.GetProvisionableResources()
                    .OfType<KeyVaultService>()
                    .Single();

                vault.Properties.Sku = new()
                {
                    Family = KeyVaultSkuFamily.A,
                    Name = KeyVaultSkuName.Standard,
                };
            });
        return keyVault;
    }
}

var keyVault = AddKeyVaultCustom(builder);

#pragma warning disable ASPIRECSHARPAPPS001
var api = builder.AddCSharpApp("api", "../Api/Api.cs")
    .WithReference(keyVault);
#pragma warning restore ASPIRECSHARPAPPS001

await builder.Build().RunAsync();
