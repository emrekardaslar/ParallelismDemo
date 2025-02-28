using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using System;
using System.Threading.Tasks;


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

    public async Task<string> RenderViewToStringAsync(object pageOrController, string viewName, object model)
    {
        ActionContext actionContext;

        if (pageOrController is PageModel pageModel)
        {
            actionContext = pageModel.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>().ActionContext;
        }
        else if (pageOrController is ControllerBase controller)
        {
            actionContext = controller.ControllerContext;
        }
        else
        {
            throw new ArgumentException("Invalid type: Expected PageModel or ControllerBase", nameof(pageOrController));
        }

        var viewResult = _viewEngine.FindView(actionContext, viewName, false);

        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"View '{viewName}' not found.");
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
