namespace Backend.Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public sealed class UrlShortenerDb : IUrlShortenerDb
{
    public static string ConnectionStringName => "UrlShortenerDb";
    private readonly string _connectionString;

    public UrlShortenerDb(string connectionString) => _connectionString = connectionString;

    private SqlConnection OpenConnection() => new SqlConnection(_connectionString);

    public async Task<long> GetLatestUrlsIdAsync(CancellationToken cancellationToken)
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

    public async Task<Url?> GetShortUrlCodeByLongUrlAsync(string longUrl, CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        var command = new CommandDefinition(
                commandText: StoredProcedureNames.GETSHORTURLCODEBYLONGURL,
                parameters: new { LongUrl = longUrl },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

        var data = await connection.QueryAsync<Url>(command);

        return data.AsList().FirstOrDefault();
    }

    public async Task<bool> InsertNewUrlAsync(long id, string longUrl, string shortUrl, DateTimeOffset? expiryDate, CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        object newUrlEntry = new
        {
            Id = id,
            LongUrl = longUrl,
            ShortUrlCode = shortUrl,
            ExpiryDate = expiryDate,
        };

        var command = new CommandDefinition(
                commandText: StoredProcedureNames.INSERTNEWURLINTOURLS,
                parameters: newUrlEntry,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

        var data = await connection.QueryAsync<int>(command);

        return data.First() > 0;
    }

    public async Task<Url?> GetLongUrlByShortUrlCode(string shortUrlCode, CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        var command = new CommandDefinition(
            commandText: StoredProcedureNames.GETLONGURLBYSHORTURLCODE,
            parameters: new { ShortUrl = shortUrlCode },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var data = await connection.QueryAsync<Url>(command);

        return data.FirstOrDefault();
    }

    public async Task<bool> EnableShortUrl(long id, DateTimeOffset? newExpiryDate, CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        var command = new CommandDefinition(
            commandText: StoredProcedureNames.ENABLESHORTURL,
            parameters: new { Id = id, ExpiryDate = newExpiryDate },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var data = await connection.QueryAsync<int>(command);

        return data.FirstOrDefault() > 0;
    }

    public async Task<bool> DisableShortUrl(long id, CancellationToken cancellationToken)
    {
        await using var connection = this.OpenConnection();

        var command = new CommandDefinition(
            commandText: StoredProcedureNames.DISABLESHORTURL,
            parameters: new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var data = await connection.QueryAsync<int>(command);

        return data.FirstOrDefault() > 0;
    }
}
