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
        var data = await _repository.GetDataAsync();

        var brochureData = new BrochureViewModel
        {
            BrochureId = "123",
            Content = "Dynamic Brochure Content"
        };

        // Render Partial View - Pass 'this' as ControllerBase
        string htmlContent = await _partialViewRenderer.RenderViewToStringAsync(this, "_BrochurePartial", brochureData);

        _scriptCollector.AppendScript("AppendFlip_v9('BCD_123', false)");
        _scriptCollector.AppendScript("window.LL.Init('BCD_123', 'src', {hheiha:1});");

        return (htmlContent, _scriptCollector.GetScripts().ToList());
    }
}
