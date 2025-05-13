using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class AuthController
{
    public AuthController()
    {

    }

    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string html = HtmlTemplates.Base("SimpleMDB", "Landin Page", "Hello World!");
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
    }
}