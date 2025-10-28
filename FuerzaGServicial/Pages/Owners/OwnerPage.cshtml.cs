using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwnerService.Domain.Entities;
using UserAccountService.Domain.Entities;

namespace FuerzaGServicial.Pages.Owners;

[Authorize(Roles = UserRoles.Manager)]
public class OwnerPage : PageModel
{
    public IEnumerable<Owner> Owners { get; set; }
    private readonly OwnerService.Application.Services.OwnerService  _ownerService;
    private readonly IDataProtector _protector;

    public OwnerPage(OwnerService.Application.Services.OwnerService ownerService, IDataProtectionProvider provider)
    {
        _ownerService = ownerService;
        _protector = provider.CreateProtector("OwnerProtector");
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
        await _ownerService.DeleteById(decryptedId);
        return RedirectToPage();
    }
}