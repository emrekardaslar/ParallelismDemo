using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    [AllowAnonymous] // Allow access without authentication
    public IActionResult MockView(string data)
    {
        return View("MockView", data);
    }
}
