using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuerzaGServicial.Pages.Owners;

public class DeleteModel : PageModel
{
    private readonly OwnerService.Application.Services.OwnerService  _ownerService;
    private readonly IDataProtector _protector;

    public DeleteModel(OwnerService.Application.Services.OwnerService ownerService, IDataProtectionProvider provider)
    {
        _ownerService = ownerService;
        _protector = provider.CreateProtector("OwnerProtector");
    }

    public void OnGet()
    { }

    public IActionResult OnPost(string id)
    {
        var decryptedId = int.Parse(_protector.Unprotect(id));
        _ownerService.DeleteById(decryptedId);
        return RedirectToPage("/Owners/OwnerPage");
    }
}