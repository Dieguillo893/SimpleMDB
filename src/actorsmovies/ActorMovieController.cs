using System.Collections;
using System.Collections.Specialized;
using System.Net;

namespace SimpleMDB;

public class ActorMovieController
{
    private ActorMovieService actorMovieService;
    private ActorService actorService;
    private MovieService movieService;


    public ActorMovieController(ActorMovieService actorMovieService, ActorService actorService, MovieService movieService)
    {
        this.actorMovieService = actorMovieService;
        this.actorService = actorService;
        this.movieService = movieService;
    }

    public ActorMovieController(MockActorMovieService actorMovieService, MockActorServices actorService, MockMovieServices movieService)
    {
        this.actorMovieService = actorMovieService;
        this.actorService = actorService;
        this.movieService = movieService;
    }


    // GET /actors/movies?aid=1&page=1&size=5
    public async Task ViewAllMoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1; // default 1
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMoviesByActor(aid, page, size);

        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            // Aquí actorMovies contiene ActorMovie, que tiene Movie y RoleName
            var actorMovies = result2.Value!;
            int totalCount = actorMovies.TotalCount;

            string html = ActorMovieHtmlTemplates.ViewAllMoviesByActor(actor, actorMovies.Values, totalCount, page, size);
            string content = HtmlTemplates.Base("SimpleMDB", "View All Movies By Actor Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);

        }
        else
        {
            string error = "";
            if (!result1.IsValid) error += result1.Error?.Message ?? "";
            if (!result2.IsValid) error += result2.Error?.Message ?? "";

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }


    // GET /movies/actors?mid=1&page=1&size=5
    public async Task ViewAllActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : -1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActorsByMovie(mid, page, size);

        if (result1.IsValid && result2.IsValid)
        {
            var movie = result1.Value!;
            var pagedResult = result2.Value!;
            var actors = pagedResult.Values;
            int totalCount = pagedResult.TotalCount;

            string html = ActorMovieHtmlTemplates.ViewAllActorsByMovie(movie, actors, totalCount, page, size);
            string content = HtmlTemplates.Base("SimpleMDB", "View All Actors By Movie Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }


    // GET /actors/movies/add?aid=1
    public async Task AddMoviesByActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMovies();

        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            var movies = result2.Value!;

            string html = ActorMovieHtmlTemplates.AddMoviesByActor(actor, movies);
            string content = HtmlTemplates.Base("SimpleMDB", "Add Movies By Actor Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = "";
            if (!result1.IsValid) error += result1.Error?.Message ?? "Error loading actor.";
            if (!result2.IsValid) error += result2.Error?.Message ?? " Error loading movies.";

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }

    // GET /movies/actors/add?aid=1
    public async Task AddActorsByMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActors();

        if (result1.IsValid && result2.IsValid)
        {
            var movies = result1.Value!;
            var actors = result2.Value!;

            string html = ActorMovieHtmlTemplates.AddActorsByMovie(movies, actors);
            string content = HtmlTemplates.Base("SimpleMDB", "Add Actors By Movies Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = "";
            if (!result1.IsValid) error += result1.Error?.Message ?? "Error loading actor.";
            if (!result2.IsValid) error += result2.Error?.Message ?? " Error loading movies.";

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }

    // POST /actors/movies/add
    public async Task AddMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();
        var aid = int.TryParse(formData["aid"], out int a) ? a : -1;
        var mid = int.TryParse(formData["mid"], out int m) ? m : -1;
        var rolename = formData["rolename"] ?? "Popo";

        var result = await actorMovieService.Create(aid, mid, rolename);

        if (result.IsValid)
        {
            var message = Uri.EscapeDataString("ActorMovie added successfully!");
            await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={aid}&message={message}");
        }
        else
        {
            var errorMessage = Uri.EscapeDataString(result.Error?.Message ?? "Unknown error");
            var rol = Uri.EscapeDataString(rolename);
            await HttpUtils.Redirect(req, res, options, $"/actors/movies/add?aid={aid}&mid={mid}&rolename={rol}&message={errorMessage}");
        }
    }

    // POST /movies/actors/add
    public async Task AddActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();
        var mid = int.TryParse(formData["mid"], out int m) ? m : -1;
        var aid = int.TryParse(formData["aid"], out int a) ? a : -1;
        var rolename = formData["rolename"] ?? "Popo";

        var result = await actorMovieService.Create(aid, mid, rolename);

        if (result.IsValid)
        {
            var message = Uri.EscapeDataString("ActorMovie added successfully!");
            await HttpUtils.Redirect(req, res, options, $"/movies/actors?mid={mid}&message={message}");
        }
        else
        {
            var errorMessage = Uri.EscapeDataString(result.Error?.Message ?? "Unknown error");
            var rol = Uri.EscapeDataString(rolename);
            await HttpUtils.Redirect(req, res, options, $"/movies/actors/add?mid={mid}&aid={aid}&rolename={rol}&message={errorMessage}");
        }
    }

    // POST /actors/movies/remove
    public async Task RemoveMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();
        int amid = int.TryParse(formData["amid"], out int a) ? a : -1;

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed successfully!");
            await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={result.Value!.ActorId}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, $"/actors");
        }
    }

    // POST /movies/actors/remove
    public async Task RemoveActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();
        int amid = int.TryParse(formData["amid"], out int a) ? a : -1;

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed successfully!");
            await HttpUtils.Redirect(req, res, options, $"/movies/actors?mid={result.Value!.MovieId}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, $"/movies");
        }
    }
}