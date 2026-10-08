// Autor: Iago Barboza Ceccatto
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var databaseType = AppDatabaseType.Sqlite;

        string connectionString;

        if (databaseType == AppDatabaseType.Sqlite)
        {
            var dbPath = DeviceInfo.Platform == DevicePlatform.WinUI
                ? @"C:\DEV\AcademiaDoZe\db_academia_do_ze.db"
                : Path.Combine(FileSystem.AppDataDirectory, "db_academia_do_ze.db");

            var dbDirectory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrWhiteSpace(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            connectionString = $"Data Source={dbPath};Default Timeout=5;";
        }
        else
        {
            const string dbServer = "localhost";
            const string dbDatabase = "db_academia_do_ze";
            const string dbUser = "root";
            const string dbPassword = "";

            string dbComplemento = string.Empty;
            if (databaseType == AppDatabaseType.SqlServer)
            {
                dbComplemento = "TrustServerCertificate=True;Encrypt=True;Connect Timeout=5;Connection Timeout=5;";
            }
            else if (databaseType == AppDatabaseType.MySql)
            {
                dbComplemento = "Connection Timeout=5;Default Command Timeout=30;";
            }

            connectionString = $"Server={dbServer};Database={dbDatabase};User Id={dbUser};Password={dbPassword};{dbComplemento}";
        }

        services.AddSingleton(new RepositoryConfig
        {
            ConnectionString = connectionString,
            DatabaseType = databaseType.ToInfrastructure()
        });

        services.AddApplicationServices();
    }
}
