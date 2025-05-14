using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class App
{
    private HttpListener server;
    private HttpRouter router;

    public App()
    {
        string host = "http://127.0.0.1:8080/";
        server = new HttpListener();

        server.Prefixes.Add(host);

        Console.WriteLine("Server listening on...." + host);

        var userRepository = new MockUserRepository();
        var userService = new MockUserServices(userRepository);
        var UserController = new UserController(userService);
        var authController = new AuthController(userService);

        router = new HttpRouter();
        router.Use(HttpUtils.ReadRequestFormData);

        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/users", UserController.ViewAllGet);
        router.AddGet("/users/add", UserController.AddGet);
        router.AddPost("/users/add", UserController.AddPost);
        router.AddGet("/users/view", UserController.ViewGet);
        router.AddGet("/users/edit", UserController.EditGet);
        router.AddPost("/users/edit", UserController.EditPost);
        router.AddGet("/users/remove", UserController.RemoveGet);
    }

    public async Task Start()
    {
        server.Start();

        while (server.IsListening)
        {
            var ctx = server.GetContext();
            await HandleContextAsync(ctx);
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

        await router.Handle(req, res, options);
    }
}