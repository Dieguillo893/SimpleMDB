using System.Collections;
using System.Net;


namespace SimpleMDB;

public class App
{
    private HttpListener server;
    private HttpRouter router;
    private int requestId;

    public App()
    {
        string host = "http://127.0.0.1:8080/";
        server = new HttpListener();

        server.Prefixes.Add(host);
        requestId = 0;

        Console.WriteLine("Server listening on...." + host);

        var userRepository = new MockUserRepository();
        var userService = new MockUserServices(userRepository);
        var UserController = new UserController(userService);
        var authController = new AuthController(userService);

        // var actorRepository = new MockActorRepository
        var actorRepository = new MySqlActorRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=2611");
        var actorService = new MockActorServices(actorRepository);
        var ActorController = new ActorController(actorService);

        //var movieRepository = new MockMovieRepository();
        var movieRepository = new MySqlMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=2611");
        var movieService = new MockMovieServices(movieRepository);
        var MovieController = new MovieController(movieService);

        //var actorMovieRepository = new MockActorMovieRepository(actorRepository, movieRepository);
        var actorMovieRepository = new MySqlActorMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=2611");
        var actorMovieService = new MockActorMovieService(actorMovieRepository);
        var actorMovieController = new ActorMovieController(actorMovieService, actorService, movieService);

        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);
        router.Use(HttpUtils.ReadRequestFormData);

        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/register", authController.RegisterGet);
        router.AddPost("/register", authController.RegisterPost);
        router.AddGet("/login", authController.LoginGet);
        router.AddPost("/login", authController.LoginPost);
        router.AddPost("/logout", authController.LogoutPost);




        router.AddGet("/users", authController.CheckAdmin, UserController.ViewAllUsersGet);
        router.AddGet("/users/add", authController.CheckAdmin, UserController.AddUserGet);
        router.AddPost("/users/add", authController.CheckAdmin, UserController.AddUserPost);
        router.AddGet("/users/view", authController.CheckAdmin, UserController.ViewUserGet);
        router.AddGet("/users/edit", authController.CheckAdmin, UserController.EditUserGet);
        router.AddPost("/users/edit", authController.CheckAdmin, UserController.EditUserPost);
        router.AddPost("/users/remove", authController.CheckAdmin, UserController.RemoveUserPost);


        router.AddGet("/actors", ActorController.ViewAllActorsGet);
        router.AddGet("/actors/add", authController.CheckAuth, ActorController.AddActorGet);
        router.AddPost("/actors/add", authController.CheckAuth, ActorController.AddActorPost);
        router.AddGet("/actors/view", authController.CheckAuth, ActorController.ViewActorGet);
        router.AddGet("/actors/edit", authController.CheckAuth, ActorController.EditActorGet);
        router.AddPost("/actors/edit", authController.CheckAuth, ActorController.EditActorPost);
        router.AddPost("/actors/remove", authController.CheckAuth, ActorController.RemoveActorPost);

        router.AddGet("/movies", MovieController.ViewAllMoviesGet);
        router.AddGet("/movies/add", authController.CheckAuth, MovieController.AddMovieGet);
        router.AddPost("/movies/add", authController.CheckAuth, MovieController.AddMoviePost);
        router.AddGet("/movies/view", authController.CheckAuth, MovieController.ViewMovieGet);
        router.AddGet("/movies/edit", authController.CheckAuth, MovieController.EditMovieGet);
        router.AddPost("/movies/edit", authController.CheckAuth, MovieController.EditMoviePost);
        router.AddPost("/movies/remove", authController.CheckAuth, MovieController.RemoveMoviePost);

        router.AddGet("/actors/movies", authController.CheckAuth, actorMovieController.ViewAllMoviesByActor);
        router.AddGet("/actors/movies/add", authController.CheckAuth, actorMovieController.AddMoviesByActorGet);
        router.AddPost("/actors/movies/add", authController.CheckAuth, actorMovieController.AddMoviesByActorPost);
        router.AddPost("/actors/movies/remove", authController.CheckAuth, actorMovieController.RemoveMoviesByActorPost);

        router.AddGet("/movies/actors", authController.CheckAuth, actorMovieController.ViewAllActorsByMovie);
        router.AddGet("/movies/actors/add", authController.CheckAuth, actorMovieController.AddActorsByMovieGet);
        router.AddPost("/movies/actors/add", authController.CheckAuth, actorMovieController.AddActorsByMoviePost);
        router.AddPost("/movies/actors/remove", authController.CheckAuth, actorMovieController.RemoveActorsByMoviePost);
    }

    public async Task Start()
    {
        server.Start();

        while (server.IsListening)
        {
            var ctx = await server.GetContextAsync();
            _ = HandleContextAsync(ctx);
        }
    }

    public void Stop()
    {
        server.Stop();
        server.Close();
    }

    private async Task HandleContextAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;
        var options = new Hashtable();

        var rid = req.Headers["X-Request-ID"] ?? requestId.ToString().PadLeft(6, ' ');
        var method = req.HttpMethod;
        var rawUrl = req.RawUrl;
        var remoteEndPoint = req.RemoteEndPoint;
        res.StatusCode = HttpRouter.RESPONSE_NOT_SENT_YET;
        DateTime startTime = DateTime.UtcNow;
        requestId++;


        try
        {
            await router.Handle(req, res, options);
        }
        catch (Exception ex)
        {
            string error = ex.ToString();

            if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {
                if (Environment.GetEnvironmentVariable("DEVELOPMENT_MODE") != "Production")
                {
                    string html = HtmlTemplates.Base("SimpleMDB", "Error Page", ex.ToString());
                    await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, html);
                }
                else
                {
                    string html = HtmlTemplates.Base("SimpleMDB", "Error Page", "An error ocurred");
                    await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, html);
                }
            }
        }
        finally
        {
            if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {
                string html = HtmlTemplates.Base("SimpleMDB", "Not Found Page", "Resource was not found.");
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.NotFound, html);
            }

            TimeSpan elapsedTime = DateTime.UtcNow - startTime;

            Console.WriteLine($"Request {rid}: {method} {rawUrl} from {req.UserHostName} --> {res.StatusCode} ({res.ContentLength64} bytes) [{res.ContentType}] in {elapsedTime.TotalMilliseconds}ms");
        }
    }
}