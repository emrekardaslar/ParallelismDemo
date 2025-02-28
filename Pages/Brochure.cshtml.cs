using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BrochureModel : PageModel
{
    private readonly ScriptCollector _scriptCollector;

    public BrochureModel(ScriptCollector scriptCollector)
    {
        _scriptCollector = scriptCollector;
    }

    public IActionResult OnGet()
    {
        // Collect JavaScript Calls
        _scriptCollector.AppendScript("AppendFlip_v9('BCD_123', false)");
        _scriptCollector.AppendScript("window.LL.Init('BCD_123', 'src', {hheiha:1});");

        // Generate HTML (this can be dynamic)
        string htmlContent = "<div id='BCD_123'><p>Brochure Content</p></div>";

        return new JsonResult(new
        {
            Html = htmlContent,
            Scripts = _scriptCollector.GetScripts()
        });
    }
}
