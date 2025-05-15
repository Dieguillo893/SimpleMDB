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

        var actorRepository = new MockActorRepository();
        var actorService = new MockActorServices(actorRepository);
        var ActorController = new ActorController(actorService);


        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);
        router.Use(HttpUtils.ReadRequestFormData);

        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/users", UserController.ViewAllUsersGet);
        router.AddGet("/users/add", UserController.AddUserGet);
        router.AddPost("/users/add", UserController.AddUserPost);
        router.AddGet("/users/view", UserController.ViewUserGet);
        router.AddGet("/users/edit", UserController.EditUserGet);
        router.AddPost("/users/edit", UserController.EditUserPost);
        router.AddPost("/users/remove", UserController.RemoveUserPost);


        router.AddGet("/actors", ActorController.ViewAllActorsGet);
        router.AddGet("/actors/add", ActorController.AddActorGet);
        router.AddPost("/actors/add", ActorController.AddActorPost);
        router.AddGet("/actors/view", ActorController.ViewActorGet);
        router.AddGet("/actors/edit", ActorController.EditActorGet);
        router.AddPost("/actors/edit", ActorController.EditActorPost);
        router.AddPost("/actors/remove", ActorController.RemoveActorPost);
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