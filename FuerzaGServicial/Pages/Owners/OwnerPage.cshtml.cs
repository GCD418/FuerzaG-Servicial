using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwnerService.Domain.Entities;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.Owners;

[Authorize(Roles = UserRoles.Manager)]
public class OwnerPage : PageModel
{
    public IEnumerable<Owner> Owners { get; set; }
    private readonly OwnerService.Application.Services.OwnerService  _ownerService;
    private readonly IDataProtector _protector;
    private readonly ISessionManager _sessionManager;

    public OwnerPage(OwnerService.Application.Services.OwnerService ownerService, 
        IDataProtectionProvider provider,
        ISessionManager sessionManager)
    {
        _ownerService = ownerService;
        _protector = provider.CreateProtector("OwnerProtector");
        _sessionManager = sessionManager;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Owners = await _ownerService.GetAll();
        return Page();
    }

    public string EncryptId(int id)
    {
        return  _protector.Protect(id.ToString());
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var decryptedId = int.Parse(_protector.Unprotect(id));
        await _ownerService.DeleteById(decryptedId, _sessionManager.UserId ?? 9999);
        return RedirectToPage();
    }
}