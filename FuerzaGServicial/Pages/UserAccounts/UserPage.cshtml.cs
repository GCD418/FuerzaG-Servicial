using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserAccountService.Domain.Entities;
using UserAccountService.Application.Services;

namespace UserAccountService.Pages.UserAccounts;

public class UserPageModel : PageModel
{
    public IEnumerable<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
    private readonly UserAccountService.Application.Services.UserAccountService _userAccountService;
    private readonly IDataProtector _protector;

    public UserPageModel(UserAccountService.Application.Services.UserAccountService userAccountService, IDataProtectionProvider provider)
    {
        _userAccountService = userAccountService;
        _protector = provider.CreateProtector("UserAccountProtector");
    }

    public async Task<IActionResult> OnGetAsync()
    {
        UserAccounts = await _userAccountService.GetAll();
        return Page();
    }

    public string ProtectId(int id)
    {
        return _protector.Protect(id.ToString());
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToPage();

        var decryptedId = int.Parse(_protector.Unprotect(id));
        await _userAccountService.DeleteById(decryptedId);
        return RedirectToPage();
    }
}
