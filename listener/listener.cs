// Workshop listener. Run from the repository root with: dotnet run listener/listener.cs
// It prints every request reaching http://localhost:8765/ so the room can watch a leak arrive.
using System.Net;

const string ListenerPrefix = "http://localhost:8765/";
const string SeparatorLine = "--------------------------------------------------";

using var listener = new HttpListener();
listener.Prefixes.Add(ListenerPrefix);
listener.Start();
Console.WriteLine($"Listening on {ListenerPrefix}");

while (true)
{
    var context = await listener.GetContextAsync();
    await PrintRequestAsync(context.Request);
    RespondEmpty(context.Response);
}

async Task PrintRequestAsync(HttpListenerRequest request)
{
    Console.WriteLine(SeparatorLine);
    Console.WriteLine($"{DateTime.Now:HH:mm:ss}  {request.HttpMethod}  {request.RawUrl}");
    PrintHeaders(request);
    await PrintBodyAsync(request);
}

void PrintHeaders(HttpListenerRequest request)
{
    foreach (var name in request.Headers.AllKeys)
    {
        Console.WriteLine($"{name}: {request.Headers[name]}");
    }
}

async Task PrintBodyAsync(HttpListenerRequest request)
{
    if (!request.HasEntityBody)
    {
        return;
    }
    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
    var body = await reader.ReadToEndAsync();
    Console.WriteLine();
    Console.WriteLine(body);
}

void RespondEmpty(HttpListenerResponse response)
{
    response.StatusCode = 200;
    response.Close();
}
