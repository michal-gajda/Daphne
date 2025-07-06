namespace Daphne.Application.FunctionalTests;

using Daphne.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

[TestClass]
public abstract class TestBase
{
    protected readonly static PostgreSqlContainer Container = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithCleanUp(true)
        .Build();

    protected ServiceProvider Provider;

    public TestBase()
    {
        var collection = new Dictionary<string, string?>()
        {
            {"ConnectionStrings:TestConnection", Container.GetConnectionString() },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(collection)
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();

        services.AddApplication();
        services.AddInfrastructure(configuration);

        this.Provider = services.BuildServiceProvider();
    }

    [AssemblyInitialize]
    public static async Task AssemblyInit(TestContext context)
    {
        await Container.StartAsync();
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanup()
    {
        await Container.DisposeAsync();
    }
}
