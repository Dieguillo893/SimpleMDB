namespace SimpleMDB;

public interface MovieRepository
{
    public Task<PagedResult<Movie>> ReadAll(int page, int size);
    public Task<Movie?> Create(Movie movie);
    public Task<Movie?> Read(int id);
    public Task<Movie?> Update(int id, Movie newMovie);
    public Task<Movie?> Delete(int id);
}
