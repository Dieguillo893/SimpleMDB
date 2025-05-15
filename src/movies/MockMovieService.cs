namespace SimpleMDB;

public class MockMovieServices : MovieService
{
    private MovieRepository movieRepository;

    public MockMovieServices(MovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    public async Task<Result<PagedResult<Movie>>> ReadAll(int page, int size)
    {
        var pagedResult = await movieRepository.ReadAll(page, size);

        var result = (pagedResult == null) ?
        new Result<PagedResult<Movie>>(new Exception("No Result found.")) :
        new Result<PagedResult<Movie>>(pagedResult);

        return await Task.FromResult(result);
    }

    public async Task<Result<Movie>> Create(Movie newMovie)
    {
        if (string.IsNullOrWhiteSpace(newMovie.Title))
        {
            return new Result<Movie>(new Exception("Title cannot be empty"));
        }
        else if (newMovie.Title.Length > 256)
        {
            return new Result<Movie>(new Exception("Title cannot have than 256 characters."));
        }
        else if (newMovie.Year > DateTime.Now.Year)
        {
            return new Result<Movie>(new Exception("Year cannot be in the future."));
        }

        Movie? movie = await movieRepository.Create(newMovie);

        var result = (movie == null) ?
        new Result<Movie>(new Exception("Movie could not be created")) :
        new Result<Movie>(movie);

        return await Task.FromResult(result);
    }

    public async Task<Result<Movie>> Read(int id)
    {
        Movie? movie = await movieRepository.Read(id);

        var result = (movie == null) ?
        new Result<Movie>(new Exception("Movie could not be created")) :
        new Result<Movie>(movie);

        return await Task.FromResult(result);
    }
    public async Task<Result<Movie>> Update(int id, Movie newMovie)
    {
        if (string.IsNullOrWhiteSpace(newMovie.Title))
        {
            return new Result<Movie>(new Exception("Title cannot be empty"));
        }
        else if (newMovie.Title.Length > 256)
        {
            return new Result<Movie>(new Exception("Title cannot have than 256 characters."));
        }
        else if (newMovie.Year > DateTime.Now.Year)
        {
            return new Result<Movie>(new Exception("Year cannot be in the future."));
        }

        Movie? movie = await movieRepository.Update(id, newMovie);

        var result = (movie == null) ?
        new Result<Movie>(new Exception("Movie could not be updated")) :
        new Result<Movie>(movie);

        return await Task.FromResult(result);
    }
    public async Task<Result<Movie>> Delete(int id)
    {
        Movie? movie = await movieRepository.Delete(id);

        var result = (movie == null) ?
        new Result<Movie>(new Exception("Movie could not be delete")) :
        new Result<Movie>(movie);

        return await Task.FromResult(result);
    }
}