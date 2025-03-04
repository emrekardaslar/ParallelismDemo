using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly MockRepository _repository;
    private readonly PartialViewRenderer _partialViewRenderer;
    private readonly ScriptCollector _scriptCollector;

    public DataController(MockRepository repository, PartialViewRenderer partialViewRenderer, ScriptCollector scriptCollector)
    {
        _repository = repository;
        _partialViewRenderer = partialViewRenderer;
        _scriptCollector = scriptCollector;
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var stopwatch = Stopwatch.StartNew();

        // Fetch component data and render the partial view
        var result = await FetchAndRenderBrochureAsync();

        stopwatch.Stop();

        return Ok(new
        {
            brochure = new
            {
                html = result.Html,
                js = result.Scripts
            },
            TimeTakenMs = stopwatch.ElapsedMilliseconds
        });
    }

    private async Task<(string Html, List<string> Scripts)> FetchAndRenderBrochureAsync()
    {
        // Start repository data fetch asynchronously
        var dataTask = _repository.GetDataAsync();

        // Prepare the view model (No need to wait for repository data)
        var brochureData = new BrochureViewModel
        {
            BrochureId = "123",
            Content = "Dynamic Brochure Content"
        };
        string brochureDivId = $"BCD_{brochureData.BrochureId}";

        // Start rendering the partial view asynchronously
        var renderTask = _partialViewRenderer.RenderViewToStringAsync(this, "_BrochurePartial", brochureData);

        // Wait for both tasks to complete in parallel
        await Task.WhenAll(dataTask, renderTask);

        // Collect JavaScript Calls
        _scriptCollector.AppendScript($"AppendFlip_v9('{brochureDivId}', false)");
        _scriptCollector.AppendScript($"window.LL.Init('{brochureDivId}', 'src', {{hheiha:1}});");

        return (renderTask.Result, _scriptCollector.GetScripts().ToList());
    }

}
