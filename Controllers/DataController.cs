using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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

        // Call the repository and render Razor page 5 times in parallel
        var tasks = Enumerable.Range(1, 5).Select(_ => FetchAndRenderDataAsync());
        var results = await Task.WhenAll(tasks);

        stopwatch.Stop();

        return Ok(new
        {
            Results = results,
            TimeTakenMs = stopwatch.ElapsedMilliseconds
        });
    }

    private async Task<string> FetchAndRenderDataAsync()
    {
        var data = await _repository.GetDataAsync();
        return await RenderRazorPageAsync(data);
    }

    private async Task<string> RenderRazorPageAsync(string data)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        return await httpClient.GetStringAsync($"http://localhost:5236/MockPage?data={data}");
    }
}
