using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly MockRepository _repository = new MockRepository();
    private readonly IHttpClientFactory _httpClientFactory;

    public DataController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var stopwatch = Stopwatch.StartNew();

        // Fetch component data
        var brochureTask = FetchAndRenderBrochureAsync();
        await Task.WhenAll(brochureTask);

        stopwatch.Stop();

        return Ok(new
        {
            brochure = new
            {
                html = brochureTask.Result.Html,
                js = brochureTask.Result.Scripts
            },
            TimeTakenMs = stopwatch.ElapsedMilliseconds
        });
    }

    private async Task<(string Html, List<string> Scripts)> FetchAndRenderBrochureAsync()
    {
        var data = await _repository.GetDataAsync();
        return await RenderRazorPageAsync("Brochure", data);
    }

    private async Task<(string Html, List<string> Scripts)> RenderRazorPageAsync(string component, string data)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetStringAsync($"http://localhost:5236/{component}");

        // Deserialize JSON response from Razor Page
        var result = JsonSerializer.Deserialize<RazorResponse>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return (result?.Html ?? "", result?.Scripts ?? new List<string>());
    }

    private class RazorResponse
    {
        public string Html { get; set; }
        public List<string> Scripts { get; set; }
    }
}
