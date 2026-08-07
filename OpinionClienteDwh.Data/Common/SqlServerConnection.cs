using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace OpinionClienteDwh.Data.Common;

public abstract class SqlServerConnection
{
    private const int TimeoutSegundos = 120;

    private readonly string _connectionString;

    protected SqlServerConnection(IConfiguration configuration, string nombreConnectionString)
    {
        _connectionString = configuration.GetConnectionString(nombreConnectionString)
            ?? throw new InvalidOperationException($"Connection string '{nombreConnectionString}' no configurada.");
    }

    protected SqlConnection GetConnection() => new(_connectionString);

    protected async Task ExecuteNonQueryAsync(
        string spName,
        Action<SqlCommand> parameters,
        CancellationToken cancellationToken = default)
    {
        using var connection = GetConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = CrearComando(connection, spName);
        parameters(command);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    protected async Task<List<T>> ExecuteReaderAsync<T>(
        string spName,
        Action<SqlCommand> parameters,
        Func<SqlDataReader, T> map,
        CancellationToken cancellationToken = default)
    {
        using var connection = GetConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = CrearComando(connection, spName);
        parameters(command);

        var data = new List<T>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            data.Add(map(reader));

        return data;
    }

    protected async Task BulkInsertAsync(
        string destinationTable,
        DataTable data,
        IEnumerable<(string Origen, string Destino)> columnas)
    {
        using var connection = GetConnection();
        await connection.OpenAsync();

        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = destinationTable,
            BulkCopyTimeout = TimeoutSegundos
        };

        foreach (var (origen, destino) in columnas)
            bulkCopy.ColumnMappings.Add(origen, destino);

        await bulkCopy.WriteToServerAsync(data);
    }

    protected static void AgregarParametroTvp(
        SqlCommand command,
        string nombreParametro,
        string nombreTipoTvp,
        IEnumerable<SqlDataRecord> registros)
    {
        var parametro = command.Parameters.AddWithValue(nombreParametro, registros);
        parametro.SqlDbType = SqlDbType.Structured;
        parametro.TypeName = nombreTipoTvp;
    }

    private static SqlCommand CrearComando(SqlConnection connection, string spName)
    {
        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = spName;
        command.CommandTimeout = TimeoutSegundos;
        return command;
    }
}