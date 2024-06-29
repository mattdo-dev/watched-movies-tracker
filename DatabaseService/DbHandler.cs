using Npgsql;
using System.Data;
using DatabaseService.Utils;

namespace DatabaseService;

public static class DbHandler
{
    private static readonly Lazy<NpgsqlConnection> Lazy;

    static DbHandler()
    {
        const string connectionString = "Host=postgresdb;" +
                                        "Port=5432;" +
                                        "Username=dlc-db;" +
                                        "Password=dlcdb-nopass;" +
                                        "Database=watched-dlc";
        Lazy = new Lazy<NpgsqlConnection>(() => new NpgsqlConnection(connectionString));

        const string testDropTableQuery = "DROP TABLE IF EXISTS watched_movies";

        ExecuteNonQuery(testDropTableQuery);

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS watched_movies (
                    id SERIAL PRIMARY KEY,
                    short_name VARCHAR(255),
                    movie_name VARCHAR(255),
                    last_watched DATE
                );";

        ExecuteNonQuery(createTableQuery);
    }

    private static NpgsqlConnection Connection => Lazy.Value;

    public static void TestConnection()
    {
        Console.WriteLine("Testing Postgres connection...");
        try
        {
            Connection.Open();
            Console.WriteLine("Connection to Postgres db successful!");
        }
        finally
        {
            Connection.Close();
        }
    }


    private static int ExecuteNonQuery(string query)
    {
        using var conn = new NpgsqlConnection(Connection.ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(query, conn);
        return cmd.ExecuteNonQuery();
    }

    public static void InsertWatchedMovie(string movie, DateTime lastSeen)
    {
        const string insertQuery = """
                                   INSERT INTO watched_movies (short_name, movie_name, last_watched) 
                                   VALUES (@short, @full, @lastSeen) 
                                   RETURNING id;
                                   """;

        var lower = NormalizeTitle.Normalize(movie);

        using var conn = new NpgsqlConnection(Connection.ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(insertQuery, conn);
        cmd.Parameters.AddWithValue("@short", lower);
        cmd.Parameters.AddWithValue("@full", movie);
        cmd.Parameters.AddWithValue("@lastSeen", lastSeen);

        cmd.ExecuteScalar();
    }

    public static void DeleteWatchedMovie(string normalizedTitle)
    {
        const string deleteQuery = """
                                   DELETE FROM watched_movies
                                   WHERE short_name = @short
                                   RETURNING id;
                                   """;

        using var conn = new NpgsqlConnection(Connection.ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(deleteQuery, conn);
        cmd.Parameters.AddWithValue("@short", normalizedTitle);

        cmd.ExecuteScalar();
    }

    public static DateTime GetMovieLastWatchedDate(string normalizedTitle)
    {
        const string searchQuery = """
                                   SELECT last_watched 
                                   FROM watched_movies
                                   WHERE short_name = @short;
                                   """;

        using var conn = new NpgsqlConnection(Connection.ConnectionString);
        conn.Open();
        // TODO: bug with the redis search algo and the text inputted here.
        normalizedTitle = normalizedTitle.ToLower();
        using var cmd = new NpgsqlCommand(searchQuery, conn);
        cmd.Parameters.AddWithValue("@short", normalizedTitle);

        var result = cmd.ExecuteScalar();

        if (result == DBNull.Value || result == null)
        {
            throw new DataException("Null value in date that should not exist...?");
        }

        return (DateTime) result;
    }

    public static string? GetFirstMovies()
    {
        const string searchQuery = """
                                   SELECT DISTINCT ON (id) *
                                   FROM watched_movies
                                   ORDER BY id, movie_name
                                   """;

        using var conn = new NpgsqlConnection(Connection.ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(searchQuery, conn);
        using var reader = cmd.ExecuteReader();

        return reader.Read() ? reader["movie_name"].ToString() : null;
    }
}
