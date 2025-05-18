namespace SimpleMDB
{
    public interface ActorMovieService
    {
        Task<Result<PagedResult<(ActorMovie, Movie)>>> ReadAllMoviesByActor(int actorId, int page, int size);
        Task<Result<PagedResult<(ActorMovie, Actor)>>> ReadAllActorsByMovie(int movieId, int page, int size);
        Task<Result<List<Actor>>> ReadAllActors();
        Task<Result<List<Movie>>> ReadAllMovies();
        Task<Result<ActorMovie>> Create(int actorId, int movieId, string rolename);
        Task<Result<ActorMovie>> Delete(int id);
    }
}
