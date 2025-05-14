using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class AuthController
{
    private UserService userService;

    public AuthController(UserService userService)
    {
        this.userService = userService;
    }


    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string html = $@"
        <nav>
          <ul>
            <li><a href=""/register"">Register</a></li>
            <li><a href=""/login"">Login</a></li>
            <li><a href=""/logout"">Logout</a></li>
            <li><a href=""/users"">Users</a></li>
            <li><a href=""/actors"">Actors</a></li>
            <li><a href=""/movies"">Movies</a></li>
        </ul>
        </nav>
        ";
        string content = HtmlTemplates.Base("SimpleMDB", "Landin Page", html);
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
    }
}