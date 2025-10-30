using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.Services;


[Authorize(Roles = "Manager,CEO")]
public class ServicePage : PageModel
{
    public IEnumerable<Service> Services { get; set; } = Enumerable.Empty<Service>();
    private readonly ServiceService.Application.Services.ServiceService _serviceService;
    private readonly IDataProtector _protector;
    private readonly ISessionManager _sessionManager;

    public ServicePage(ServiceService.Application.Services.ServiceService serviceService, 
        IDataProtectionProvider provider,
        ISessionManager sessionManager)
    {
        _serviceService = serviceService;
        _protector = provider.CreateProtector("ServiceProtector");
        _sessionManager = sessionManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Services = await _serviceService.GetAll();
        return Page();
    }
    
    public string EncryptId(int id)
    {
        return _protector.Protect(id.ToString());
    }
    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var decryptedId = int.Parse(_protector.Unprotect(id));
        await _serviceService.DeleteById(decryptedId, _sessionManager.UserId ?? 9999);
        return RedirectToPage();
    }

}

