using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace SimpleMDB;

public class MockUserServices : UserService
{
    private UserRepository userRepository;

    public MockUserServices(UserRepository userRepository)
    {
        this.userRepository = userRepository;
        _ = Create(new User(0, "Admin", "masterofthegalaxy", "", Roles.ADMIN));
    }

    public async Task<Result<PagedResult<User>>> ReadAll(int page, int size)
    {
        var pagedResult = await userRepository.ReadAll(page, size);

        var result = (pagedResult == null) ?
        new Result<PagedResult<User>>(new Exception("No Result found.")) :
        new Result<PagedResult<User>>(pagedResult);

        return result;
    }

    public async Task<Result<User>> Create(User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }

        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannoy have than 16 characters."));
        }
        else if (await userRepository.GetUserByUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username already taken. Choose another username"));
        }

        if (string.IsNullOrWhiteSpace(newUser.Password))
        {
            return new Result<User>(new Exception("Password cannot be empty"));
        }
        // else if (newUser.Password.Length < 16)
        {
            // return new Result<User>(new Exception("Password cannoy have less than 16 characters."));
        }

        if (!Roles.IsValid(newUser.Role))
        {
            return new Result<User>(new Exception("Role is not valid"));
        }


        newUser.Salt = Path.GetRandomFileName();
        newUser.Password = Encode(newUser.Password + newUser.Salt);
        User? user = await userRepository.Create(newUser);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be created")) :
        new Result<User>(user);

        return result;
    }

    public async Task<Result<User>> Read(int id)
    {
        User? user = await userRepository.Read(id);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be created")) :
        new Result<User>(user);

        return result;
    }
    public async Task<Result<User>> Update(int id, User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }

        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannoy have than 16 characters."));
        }
        else if (await userRepository.GetUserByUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username already taken. Choose another username"));
        }

        if (string.IsNullOrWhiteSpace(newUser.Password))
        {
            return new Result<User>(new Exception("Password cannot be empty"));
        }
        else if (newUser.Password.Length < 16)
        {
            return new Result<User>(new Exception("Password cannoy have less than 16 characters."));
        }

        if (!Roles.IsValid(newUser.Role))
        {
            return new Result<User>(new Exception("Role is not valid"));
        }


        newUser.Salt = Path.GetRandomFileName();
        newUser.Password = Encode(newUser.Password + newUser.Salt);

        User? user = await userRepository.Update(id, newUser);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be updated")) :
        new Result<User>(user);

        return result;
    }
    public async Task<Result<User>> Delete(int id)
    {
        User? user = await userRepository.Delete(id);

        var result = (user == null) ?
        new Result<User>(new Exception("User could not be delete")) :
        new Result<User>(user);

        return result;
    }

    public async Task<Result<string>> GetToken(string username, string password)
    {
        User? user = await userRepository.GetUserByUsername(username);

        if (user != null && string.Equals(user.Password, Encode(password + user.Salt)))
        {
            return new Result<string>(Encode($"username={user.Username}&role={user.Role}&expires={DateTime.Now.AddMinutes(60)}"));
        }
        else
        {
            return new Result<string>(new Exception("Invalid username or password."));
        }
    }


    public async Task<Result<NameValueCollection>> ValidateToken(string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            NameValueCollection? claims = HttpUtility.ParseQueryString(Decode(token));


            // if(claims["expires"] <DateTime.Now) { // send null}
            return new Result<NameValueCollection>(claims);
        }
        else
        {
            var result = new Result<NameValueCollection>(new Exception("Invalid token"));
            return await Task.FromResult(result);
        }
    }
    public static string Encode(string plaintext)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
    }

    public static string Decode(string cyphertext)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(cyphertext));
    }
}