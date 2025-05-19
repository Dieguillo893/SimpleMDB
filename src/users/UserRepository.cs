namespace SimpleMDB;

public interface UserRepository
{
    public Task<PagedResult<User>> ReadAll(int page, int size);
    public Task<User?> Create(User user);
    public Task<User?> Read(int id);
    public Task<User?> Update(int id, User newUser);
    public Task<User?> Delete(int id);
    public Task<User?> GetUserByUsername(string username);
}
