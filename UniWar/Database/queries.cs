using Npgsql;


public class DatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CreatePartitaTable()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS partita (
                    id SERIAL PRIMARY KEY,
                    nome VARCHAR(100) NOT NULL,
                    data DATE NOT NULL
                )", connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
