using System.Data;
using MySql.Data.MySqlClient;

namespace SimpleMDB;

public class MySqlActorMovieRepository : ActorMovieRepository
{
    private string connectionString;

    public MySqlActorMovieRepository(string connectionString)
    {
        this.connectionString = connectionString;
        //Init();
    }

    private void Init()
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS ActorsMovies
        (
            id INT AUTO_INCREMENT PRIMARY KEY,
            actorId INT NOT NULL,
            movieId INT NOT NULL,
            rolename NVARCHAR(64),
            FOREIGN KEY(actorId) REFERENCES Actors(id) ON DELETE CASCADE,
            FOREIGN KEY(movieId) REFERENCES Movies(id) ON DELETE CASCADE
        )";

        cmd.ExecuteNonQuery();
    }

    public MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int size)
    {
        using var dbc = OpenDb();
        using var countCmd = dbc.CreateCommand();

        countCmd.CommandText = @"
        SELECT COUNT(*) FROM ActorsMovies as am
        JOIN Movies as m ON am.movieId = m.id
        WHERE am.actorId = @actorId
        ";

        countCmd.Parameters.AddWithValue("@actorId", actorId);

        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        SELECT * FROM ActorsMovies as am
        JOIN Movies as m ON am.movieId = m.id
        WHERE am.actorId = @actorId
        LIMIT @offset, @limit
        ";
        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amms = new List<(ActorMovie, Movie)>();

        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
                rows.GetInt32(0),   // id
                rows.GetInt32(1),   // actorId
                rows.GetInt32(2),   // movieId
                rows.GetString(3)); // rolename

            Movie m = new Movie(
                rows.GetInt32(4),   // id
                rows.GetString(5),  // title
                rows.GetInt32(6),   // year
                rows.GetString(7),  // description
                rows.GetFloat(8)    // rating
            );
            amms.Add((am, m));
        }

        return new PagedResult<(ActorMovie, Movie)>(amms, totalCount);
    }

    public async Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        using var dbc = OpenDb();
        using var countCmd = dbc.CreateCommand();

        countCmd.CommandText = @"
        SELECT COUNT(*) FROM ActorsMovies as am
        JOIN Actors as a ON am.actorId = a.id
        WHERE am.movieId = @movieId
        ";

        countCmd.Parameters.AddWithValue("@movieId", movieId);

        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        SELECT * FROM ActorsMovies as am
        JOIN Actors as a ON am.actorId = a.id
        WHERE am.movieId = @movieId
        LIMIT @offset, @limit
        ";
        cmd.Parameters.AddWithValue("@movieId", movieId);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amas = new List<(ActorMovie, Actor)>();

        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
                rows.GetInt32(0),   // id
                rows.GetInt32(1),   // actorId
                rows.GetInt32(2),   // movieId
                rows.GetString(3)); // rolename

            Actor a = new Actor(
                rows.GetInt32(4),   // id
                rows.GetString(5),  // name
                rows.GetString(6),  // birthdate
                rows.GetString(7),  // biography
                rows.GetFloat(8)    // popularity
            );
            amas.Add((am, a));
        }

        return new PagedResult<(ActorMovie, Actor)>(amas, totalCount);
    }

    public async Task<List<Actor>> ReadAllActors()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"SELECT * FROM Actors";

        using var rows = await cmd.ExecuteReaderAsync();

        var actors = new List<Actor>();

        while (await rows.ReadAsync())
        {
            Actor a = new Actor(
                rows.GetInt32(0),
                rows.GetString(1),
                rows.GetString(2),
                rows.GetString(3),
                rows.GetFloat(4)
            );
            actors.Add(a);
        }

        return actors;
    }

    public async Task<List<Movie>> ReadAllMovies()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"SELECT * FROM Movies";

        using var rows = await cmd.ExecuteReaderAsync();

        var movies = new List<Movie>();

        while (await rows.ReadAsync())
        {
            Movie m = new Movie(
                rows.GetInt32(0),
                rows.GetString(1),
                rows.GetInt32(2),
                rows.GetString(3),
                rows.GetFloat(4)
            );
            movies.Add(m);
        }

        return movies;
    }

    public async Task<ActorMovie?> Create(int actorId, int movieId, string roleName)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO ActorsMovies (actorId, movieId, rolename)
        VALUES (@actorId, @movieId, @rolename);
        SELECT LAST_INSERT_ID();
        ";

        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@movieId", movieId);
        cmd.Parameters.AddWithValue("@rolename", roleName);

        int id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return new ActorMovie(id, actorId, movieId, roleName);
    }

    public async Task<ActorMovie?> Delete(int id)
    {
        using var dbc = OpenDb();

        var actorMovie = await Read(id);
        if (actorMovie == null) return null;

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"DELETE FROM ActorsMovies WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", id);

        return (await cmd.ExecuteNonQueryAsync() > 0) ? actorMovie : null;
    }

    public async Task<ActorMovie?> Read(int id)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"SELECT * FROM ActorsMovies WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();

        if (await rows.ReadAsync())
        {
            return new ActorMovie(
                rows.GetInt32(0),
                rows.GetInt32(1),
                rows.GetInt32(2),
                rows.GetString(3)
            );
        }
        return null;
    }
}
