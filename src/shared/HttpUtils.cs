using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class HttpUtils
{
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
}