using System.Collections;
using System.Collections.Specialized;
using System.Net;

namespace SimpleMDB;

public class ActorController
{
    private readonly ActorService actorService;

    public ActorController(ActorService actorService)
    {
        this.actorService = actorService;
    }

    public async Task ViewAllActorsGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out var p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out var s) ? s : 5;
        var result = await actorService.ReadAll(page, size);

        if (result.IsValid)
        {
            var paged = result.Value!;
            var html = ActorHtmlTemplates.ViewAllActorsGet(paged.Values, paged.TotalCount, page, size);
            await HttpUtils.Respond(req, res, options, 200, HtmlTemplates.Base("SimpleMDB", "Actors", html, req.QueryString["message"] ?? ""));
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }


    // GET /actors/add
    public async Task AddActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string firstname = req.QueryString["firstname"] ?? "";
        string lastname = req.QueryString["lastname"] ?? "";
        string bio = req.QueryString["bio"] ?? "";
        string rating = req.QueryString["rating"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string html = ActorHtmlTemplates.AddActorGet(firstname, lastname, bio, rating);
        string content = HtmlTemplates.Base("SimpleMDB", "Actors Add Page", html, message);

        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);

    }

    // POST /actors/add
    public async Task AddActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string firstname = formData["firstname"] ?? "";
        string lastname = formData["lastname"] ?? "";
        string bio = formData["bio"] ?? "";
        float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

        Actor newActor = new Actor(0, firstname, lastname, bio, rating);

        Result<Actor> result = await actorService.Create(newActor);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor added sucesfully!");
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);

            await HttpUtils.Redirect(req, res, options, "/actors/add");
        }
    }

    // GET /actors/view?aid=1
    public async Task ViewActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            Actor actor = result.Value!;

            string html = ActorHtmlTemplates.ViewActorGet(actor);
            string content = HtmlTemplates.Base("SimpleMDB", "Actors View Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);

        }

    }

    //GET /actors/edit?aid=1
    public async Task EditActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            Actor actor = result.Value!;

            string html = ActorHtmlTemplates.EditActorGet(aid, actor);
            string content = HtmlTemplates.Base("SimpleMDB", "Actors Edit Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
    }

    // POST /actors/edit?aid=1
    public async Task EditActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string firstname = formData["firstname"] ?? "";
        string lastname = formData["lastname"] ?? "";
        string bio = formData["bio"] ?? "";
        float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

        Actor newActor = new Actor(0, firstname, lastname, "", rating);

        Result<Actor> result = await actorService.Update(aid, newActor);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor edited succesfully!");
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
        else
        {

            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

            await HttpUtils.Redirect(req, res, options, $"/actors/edit?aid={aid}");
        }
    }

    // Post /actors/remove?aid=1
    public async Task RemoveActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        Result<Actor> result = await actorService.Delete(aid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor remove succesfully!");
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
    }
}
