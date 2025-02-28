using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class BrochureModel : PageModel
{
    private readonly ScriptCollector _scriptCollector;
    private readonly PartialViewRenderer _partialViewRenderer;

    public BrochureModel(ScriptCollector scriptCollector, PartialViewRenderer partialViewRenderer)
    {
        _scriptCollector = scriptCollector;
        _partialViewRenderer = partialViewRenderer;
    }

    public async Task<IActionResult> OnGet()
    {
        // Fetch dynamic content (from DB, API, etc.)
        var brochureData = new BrochureViewModel
        {
            BrochureId = "123",
            Content = "Dynamic Brochure Content"
        };

        // Render Brochure Partial View
        string htmlContent = await _partialViewRenderer.RenderViewToStringAsync(this, "_BrochurePartial", brochureData);

        // Collect JavaScript Calls
        _scriptCollector.AppendScript("AppendFlip_v9('BCD_123', false)");
        _scriptCollector.AppendScript("window.LL.Init('BCD_123', 'src', {hheiha:1});");

        return new JsonResult(new
        {
            Html = htmlContent,
            Scripts = _scriptCollector.GetScripts()
        });
    }
}
