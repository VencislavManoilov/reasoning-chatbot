using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PostgreSqlDistributedCache : IDistributedCache
{
    private readonly string? _connectionString;

    public PostgreSqlDistributedCache(IConfiguration configuration)
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    }

    public byte[] Get(string key)
    {
        return GetAsync(key).GetAwaiter().GetResult() ?? Array.Empty<byte>();
    }

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);

        var command = new NpgsqlCommand("SELECT value FROM session_data WHERE id = @id AND expires_at > NOW()", connection);
        command.Parameters.AddWithValue("@id", key);

        var result = await command.ExecuteScalarAsync(token);
        return result as byte[] ?? Array.Empty<byte>();
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        SetAsync(key, value, options).GetAwaiter().GetResult();
    }

    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);

        var command = new NpgsqlCommand(@"
            INSERT INTO session_data (id, value, expires_at)
            VALUES (@id, @value, @expires_at)
            ON CONFLICT (id) 
            DO UPDATE SET value = @value, expires_at = @expires_at", connection);

        command.Parameters.AddWithValue("@id", key);
        command.Parameters.AddWithValue("@value", value);
        command.Parameters.AddWithValue("@expires_at", DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(20)));

        await command.ExecuteNonQueryAsync(token);
    }

    public void Refresh(string key)
    {
        RefreshAsync(key).GetAwaiter().GetResult();
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);

        var command = new NpgsqlCommand(@"
            UPDATE session_data 
            SET expires_at = NOW() + INTERVAL '20 minutes'
            WHERE id = @id", connection);

        command.Parameters.AddWithValue("@id", key);
        await command.ExecuteNonQueryAsync(token);
    }

    public void Remove(string key)
    {
        RemoveAsync(key).GetAwaiter().GetResult();
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);

        var command = new NpgsqlCommand("DELETE FROM session_data WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", key);
        await command.ExecuteNonQueryAsync(token);
    }
}