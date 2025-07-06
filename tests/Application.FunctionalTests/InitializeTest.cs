namespace Daphne.Application.FunctionalTests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Shouldly;

[TestClass]
public sealed class InitializeTest : TestBase
{
    [TestMethod]
    public async ValueTask InitializeTestcontainer()
    {
        var connectionString = Container.GetConnectionString();
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT version();";

        var version = (string?)await command.ExecuteScalarAsync();

        version.ShouldNotBeNullOrEmpty();
        version.ShouldContain("17");
    }

    [TestMethod]
    public async ValueTask InitializeApplication()
    {
        var configuration = this.Provider.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("TestConnection");
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT version();";

        var version = (string?)await command.ExecuteScalarAsync();

        version.ShouldNotBeNullOrEmpty();
        version.ShouldContain("17");
    }
}
