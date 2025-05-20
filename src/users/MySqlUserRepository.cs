using System.Data;
using MySql.Data.MySqlClient;

namespace SimpleMDB;

public class MySqlUserRepository : UserRepository
{
    private string connectionString;

    public MySqlUserRepository(string connectionString)
    {
        this.connectionString = connectionString;
        //Init();
    }

    private void Init()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
       CREATE TABLE IF NOT EXISTS Users
       (
        id INT AUTO_INCREMENT PRIMARY KEY,
        username NVARCHAR(64) NOT NULL UNIQUE,
        password NVARCHAR(64) NOT NULL,
        salt NVARCHAR(64) NOT NULL,
        role ENUM('Admin', 'User') NOT NULL
       )";

        cmd.ExecuteNonQuery();
    }

    public MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<PagedResult<User>> ReadAll(int page, int size)
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Users";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        int offset = (page - 1) * size;
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Users LIMIT @offset, @size";
        cmd.Parameters.AddWithValue("@offset", offset);
        cmd.Parameters.AddWithValue("@size", size);

        using var rows = await cmd.ExecuteReaderAsync();
        var users = new List<User>();

        while (await rows.ReadAsync())
        {
            users.Add(new User
            {
                Id = rows.GetInt32("id"),
                Username = rows.GetString("username"),
                Password = rows.GetString("password"),
                Salt = rows.IsDBNull("salt") ? "" : rows.GetString("salt"),
                Role = rows.GetString("role")
            });
        }

        return new PagedResult<User>(users, totalCount);
    }

    public async Task<User?> Create(User user)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO Users (username, password, salt, role)
        VALUES (@username, @password, @salt, @role);
        SELECT LAST_INSERT_ID()";
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@salt", user.Salt);
        cmd.Parameters.AddWithValue("@role", user.Role);

        user.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        return user;
    }

    public async Task<User?> Read(int id)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();

        if (await rows.ReadAsync())
        {
            return new User
            {
                Id = rows.GetInt32("id"),
                Username = rows.GetString("username"),
                Password = rows.GetString("password"),
                Salt = rows.IsDBNull("salt") ? "" : rows.GetString("salt"),
                Role = rows.GetString("role")
            };
        }

        return null;
    }

    public async Task<User?> Update(int id, User newUser)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            UPDATE Users SET 
            username = @username, 
            password = @password, 
            salt = @salt, 
            role = @role 
            WHERE id = @id";

        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@username", newUser.Username);
        cmd.Parameters.AddWithValue("@password", newUser.Password);
        cmd.Parameters.AddWithValue("@salt", newUser.Salt);
        cmd.Parameters.AddWithValue("@role", newUser.Role);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? newUser : null;
    }

    public async Task<User?> Delete(int id)
    {
        using var dbc = OpenDb();

        var user = await Read(id);
        if (user == null) return null;

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "DELETE FROM Users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? user : null;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Users WHERE username = @username";
        cmd.Parameters.AddWithValue("@username", username);

        using var rows = await cmd.ExecuteReaderAsync();

        if (await rows.ReadAsync())
        {
            return new User
            {
                Id = rows.GetInt32("id"),
                Username = rows.GetString("username"),
                Password = rows.GetString("password"),
                Salt = rows.IsDBNull("salt") ? "" : rows.GetString("salt"),
                Role = rows.GetString("role")
            };
        }

        return null;
    }
}
