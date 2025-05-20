namespace SimpleMDB;

public class MockMovieRepository : MovieRepository
{
    private List<Movie> movies;
    private int idCount;

    public MockMovieRepository()
    {
        movies = new List<Movie>();
        idCount = 0;
        var titles = new[]
        {
            "The Horizon", "Shadowfall", "Echoes of Time", "Lost in Light",
            "Crimson Sky", "Whispers", "The Forgotten Path", "Beyond Silence",
            "Twilight Road", "Dreambound", "Iron Will", "The Silent Code"
        };

        var descriptions = new[]
        {
            "A thrilling adventure through time and space.",
            "An emotional journey of self-discovery.",
            "A gripping tale of love and betrayal.",
            "An epic battle between good and evil.",
            "A suspenseful mystery with a shocking twist.",
            "A coming-of-age story set in the 1980s.",
            "A heartwarming story of hope and courage.",
            "A science fiction saga with stunning visuals.",
            "A haunting look at a dystopian future.",
            "A lighthearted comedy full of charm."
        };

        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            string title = titles[random.Next(titles.Length)] + " " + (char)('A' + random.Next(0, 26));
            int year = random.Next(1980, 2024);
            string description = descriptions[random.Next(descriptions.Length)];
            float rating = (float)(random.NextDouble() * 10);

            movies.Add(new Movie(idCount++, title, year, description, rating));
        }

        foreach (var movie in movies.GetRange(0, 5))
        {
            Console.WriteLine($"{movie.Title} ({movie.Year}) - {movie.Description} Rating: {movie.Rating:F1}");
        }
    }

    public async Task<PagedResult<Movie>> ReadAll(int page, int size)
    {
        int totalCount = movies.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);
        List<Movie> values = movies.GetRange(start, length);
        var pagedResult = new PagedResult<Movie>(values, totalCount);

        return await Task.FromResult(pagedResult);
    }

    public async Task<Movie?> Create(Movie movie)
    {
        movie.Id = idCount++;
        movies.Add(movie);
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Read(int id)
    {
        Movie? movie = movies.FirstOrDefault(u => u.Id == id);
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Update(int id, Movie newMovie)
    {
        Movie? movie = movies.FirstOrDefault(u => u.Id == id);

        if (movie != null)
        {
            movie.Title = newMovie.Title;
            movie.Year = newMovie.Year;
            movie.Description = newMovie.Description;
            movie.Rating = newMovie.Rating;
        }

        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Delete(int id)
    {
        Movie? movie = movies.FirstOrDefault(u => u.Id == id);

        if (movie != null)
        {
            movies.Remove(movie);
        }

        return await Task.FromResult(movie);
    }
}
