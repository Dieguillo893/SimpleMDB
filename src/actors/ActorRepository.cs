namespace SimpleMDB;

public interface ActorRepository
{
    public Task<PagedResult<Actor>> ReadAll(int page, int size);
    public Task<Actor?> Create(Actor actor);
    public Task<Actor?> Read(int id);
    public Task<Actor?> Update(int id, Actor newActor);
    public Task<Actor?> Delete(int id);
}
