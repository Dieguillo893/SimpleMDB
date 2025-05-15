using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace SimpleMDB;

public class HttpUtils
{
    public static void AddOptions(Hashtable options, string name, string key, string value)
    {
        var prop = (NameValueCollection?)options[name] ?? [];
        options[name] = prop;

        prop[key] = value;
    }
    public static void AddOptions(Hashtable options, string name, NameValueCollection entries)
    {
        var prop = (NameValueCollection?)options[name] ?? [];
        options[name] = prop;

        prop.Add(entries);
    }
    public static async Task Respond(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int StatusCode, string body)
    {
        byte[] content = Encoding.UTF8.GetBytes(body);
        res.StatusCode = StatusCode;
        res.ContentEncoding = Encoding.UTF8;
        res.ContentType = "text/html";
        res.ContentLength64 = content.LongLength;
        await res.OutputStream.WriteAsync(content);
        res.Close();
    }

    public static async Task Redirect(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, string location)
    {
        var redirectProps = (NameValueCollection?)options["redirect"] ?? [];
        var query = new List<string>();
        var append = location.Contains('?') ? '&' : '?';
        foreach (var key in redirectProps.AllKeys)
        {
            query.Add($"{key}={HttpUtility.UrlEncode(key)}=(redirectProps[key])");
        }

        res.Redirect(location + append + string.Join('&', query));
        res.Close();

        await Task.CompletedTask;
    }

    public static async Task ReadRequestFormData(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string type = req.ContentType ?? "";

        if (type.StartsWith("application/x-www-form-urlencoded"))
        {
            using var sr = new StreamReader(req.InputStream, Encoding.UTF8);
            string body = await sr.ReadToEndAsync();
            var formData = HttpUtility.ParseQueryString(body);

            options["req.form"] = formData;
        }

    }

    public static readonly NameValueCollection SUPPORTED_IANA_MIME_TYPES = new()
    {
        {".css", "text/css"},
        {".js", "text/javascript"},
    };

    public static async Task ServeStaticFile(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string fileName = req.Url!.AbsolutePath ?? "";
        string filePath = Path.Combine(Environment.CurrentDirectory, "static", fileName.Trim('/', '\\'));
        string fullPath = Path.GetFullPath(filePath);

        if (File.Exists(fullPath))
        {
            string ext = Path.GetExtension(fullPath);
            string type = SUPPORTED_IANA_MIME_TYPES[ext] ?? "application/octet-stream";
            using var fs = File.OpenRead(fullPath);

            res.StatusCode = (int)HttpStatusCode.OK;
            res.ContentType = type;
            res.ContentLength64 = fs.Length;

            await fs.CopyToAsync(res.OutputStream);
            res.Close();
        }
    }

    internal static void AddOptions(Hashtable options, string v, object formData)
    {
        throw new NotImplementedException();
    }
}