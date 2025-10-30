using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuerzaGServicial.Pages.Reports;

[Authorize(Roles = "Manager,CEO")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
