// Autor: Iago Barboza Ceccatto
using AcademiaDoZe.Infrastructure.Exceptions;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbProvider
{
    public const int DefaultCommandTimeout = 30;

    public static void ValidarDatabaseType(DatabaseType databaseType)
    {
        if (databaseType != DatabaseType.Sqlite)
            throw new InfrastructureException("GERENCIADOR_NAO_SUPORTADO", $"Gerenciador de banco de dados '{databaseType}' não suportado. Esta solução utiliza exclusivamente o SQLite (DatabaseType.Sqlite).");
    }

    public static DbConnection CreateConnection(string connectionString, DatabaseType databaseType = DatabaseType.Sqlite)
    {
        ValidarDatabaseType(databaseType);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InfrastructureException("CONEXAO_STRING_VAZIA", "String de conexão não pode ser vazia.");

        try
        {
            return new SqliteConnection(connectionString);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("FALHA_CONEXAO", "Falha ao instanciar conexão com o SQLite.", ex);
        }
    }

    public static DbCommand CreateCommand(string commandText, DbConnection connection, CommandType commandType = CommandType.Text)
    {
        if (connection == null) throw new InfrastructureException("CONEXAO_NULA", "Conexão não pode ser nula para criar um comando.");
        if (string.IsNullOrWhiteSpace(commandText)) throw new InfrastructureException("COMANDO_TEXTO_VAZIO", "Texto do comando não pode ser vazio.");

        try
        {
            var command = connection.CreateCommand() ?? throw new InfrastructureException("FALHA_CRIAR_COMANDO", "Falha ao criar o comando no banco de dados.");
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.CommandTimeout = DefaultCommandTimeout;
            return command;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("FALHA_CRIAR_COMANDO", "Falha ao criar o comando no banco de dados.", ex);
        }
    }

    public static DbParameter AddParameter(this DbCommand command, string name, object? value, DbType dbType)
    {
        if (command == null) throw new InfrastructureException("COMANDO_NULO", "Comando não pode ser nulo para criar parâmetro.");
        if (string.IsNullOrWhiteSpace(name)) throw new InfrastructureException("PARAMETRO_NOME_VAZIO", "Nome do parâmetro não pode ser vazio.");

        try
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            parameter.DbType = dbType;
            command.Parameters.Add(parameter);
            return parameter;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("ERRO_CRIAR_PARAMETRO", "Erro ao criar parâmetro no banco de dados.", ex);
        }
    }

    public static async Task<int> ExecuteScalarIdAsync(this DbCommand command, string errorCode = "ERRO_OBTER_ID", string errorMessage = "Falha ao obter ID inserido no banco de dados.", CancellationToken cancellationToken = default)
    {
        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32(result);
        }
        throw new InfrastructureException(errorCode, errorMessage);
    }

    public static string GetScriptName() => "script_sqlite.sql";

    public static string FormatInsertQuery(string insertSql)
    {
        if (string.IsNullOrWhiteSpace(insertSql)) throw new InfrastructureException("SQL_INSERT_VAZIO", "Comando SQL de INSERT não pode ser vazio.");
        return $"{insertSql}; SELECT last_insert_rowid();";
    }

    public static string GetCurrentDateFunction() => "DATE('now')";

    public static string GetDateAddDaysExpression(string dateExpr, string daysParam) =>
        $"DATE({dateExpr}, '+' || {daysParam} || ' days')";

    public static string GetDateHourExpression(string dateColumn) =>
        $"CAST(strftime('%H', {dateColumn}) AS INTEGER)";

    public static string GetDateMonthExpression(string dateColumn) =>
        $"CAST(strftime('%m', {dateColumn}) AS INTEGER)";

    public static string GetDateDayExpression(string dateColumn) =>
        $"CAST(strftime('%d', {dateColumn}) AS INTEGER)";
}
