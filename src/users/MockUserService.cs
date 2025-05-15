namespace SimpleMDB;

public class MockUserServices : UserService
{
    private UserRepository userRepository;

    public MockUserServices(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<Result<PagedResult<User>>> ReadAll(int page, int size)
    {
        var pagedResult = await userRepository.ReadAll(page, size);

        var result = (pagedResult == null) ?
        new Result<PagedResult<User>>(new Exception("No Result found.")) :
        new Result<PagedResult<User>>(pagedResult);

        return await Task.FromResult(result);
    }

    public async Task<Result<User>> Create(User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannoy have than 16 characters."));
        }

        User? user = await userRepository.Create(newUser);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be created")) :
        new Result<User>(user);

        return await Task.FromResult(result);
    }

    public async Task<Result<User>> Read(int id)
    {
        User? user = await userRepository.Read(id);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be created")) :
        new Result<User>(user);

        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Update(int id, User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannoy have than 16 characters."));
        }
        User? user = await userRepository.Update(id, newUser);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be updated")) :
        new Result<User>(user);

        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Delete(int id)
    {
        User? user = await userRepository.Delete(id);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be delete")) :
        new Result<User>(user);

        return await Task.FromResult(result);
    }
}