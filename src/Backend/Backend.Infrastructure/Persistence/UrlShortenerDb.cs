using Backend.Application.Persistence;
using Microsoft.Data.SqlClient;
using Dapper;
using Backend.Domain.Constants;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public sealed class UrlShortenerDb : IUrlShortenerDb
{
    public static string ConnectionStringName => "UrlShortenerDb";
    private readonly string _connectionString;

    public UrlShortenerDb(string connectionString) => _connectionString = connectionString;

    private SqlConnection OpenConnection() => new SqlConnection(_connectionString);

    public async Task<long> GetLatestUrlsId(CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        var command = new CommandDefinition(
                commandText: StoredProcedureNames.GETLATESTURLSID,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

        var data = await connection.QueryAsync<long>(command);

        return data.AsList().FirstOrDefault();
    }

    public async Task<string?> GetShortUrlCodeByLongUrl(string longUrl, CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        var command = new CommandDefinition(
                commandText: StoredProcedureNames.GETSHORTURLCODEBYLONGURL,
                parameters: new { LongUrl = longUrl },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

        var data = await connection.QueryAsync<string>(command);

        return data.AsList().FirstOrDefault();
    }

    public async Task<bool> InsertNewUrl(long id, string longUrl, string shortUrl, CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        var command = new CommandDefinition(
                commandText: StoredProcedureNames.INSERTNEWURLINTOURLS,
                parameters: new { Id = id, LongUrl = longUrl, ShortUrlCode = shortUrl },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

        var data = await connection.QueryAsync<int>(command);

        return data.First() > 0;
    }
}
