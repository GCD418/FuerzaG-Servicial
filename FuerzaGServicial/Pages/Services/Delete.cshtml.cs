using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuerzaGServicial.Pages.Services;

[Authorize(Roles = "Manager")]
public class DeleteModel : PageModel
{
    private readonly ServiceService.Application.Services.ServiceService _serviceService;
    private readonly IDataProtector _protector;

    public DeleteModel(
        ServiceService.Application.Services.ServiceService serviceService,
        IDataProtectionProvider provider)
    {
        _serviceService = serviceService;
        _protector = provider.CreateProtector("ServiceProtector");
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var decryptedId = int.Parse(_protector.Unprotect(id));
        await _serviceService.DeleteById(decryptedId);
        return RedirectToPage("/Services/ServicePage");
    }
}