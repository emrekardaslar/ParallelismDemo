using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class PartialViewRenderer
{
    private readonly ICompositeViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public PartialViewRenderer(
        ICompositeViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderViewToStringAsync(PageModel pageModel, string viewName, object model)
    {
        var actionContext = pageModel.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>().ActionContext;

        var viewResult = _viewEngine.FindView(actionContext, viewName, false);

        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"Could not find view: {viewName}");
        }

        using var sw = new StringWriter();
        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}
