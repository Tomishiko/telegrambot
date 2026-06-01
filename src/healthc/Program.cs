string? path = Environment.GetEnvironmentVariable("HEALTH_CHECK_PATH");
if (string.IsNullOrEmpty(path))
    path = "/health";
using var http = new HttpClient();
HttpResponseMessage response;
try
{
    response = await http.GetAsync($"http://localhost:80{path}");
    return response.IsSuccessStatusCode ? 0 : 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}

