using Microsoft.AspNetCore.Mvc.RazorPages;

public class MockPageModel : PageModel
{
    public string Data { get; set; } = string.Empty;

    public void OnGet(string data)
    {
        Data = data;
    }
}
