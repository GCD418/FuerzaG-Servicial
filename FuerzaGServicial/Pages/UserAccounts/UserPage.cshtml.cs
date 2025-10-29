using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.UserAccounts;

[Authorize(Roles = UserRoles.CEO)]
public class UserPageModel : PageModel
{
    public IEnumerable<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
    private readonly UserAccountService.Application.Services.UserAccountService _userAccountService;
    private readonly IDataProtector _protector;
    private readonly ISessionManager _sessionManager;

    public UserPageModel(UserAccountService.Application.Services.UserAccountService userAccountService,
        IDataProtectionProvider provider,
        ISessionManager sessionManager)
    {
        _userAccountService = userAccountService;
        _protector = provider.CreateProtector("UserAccountProtector");
        _sessionManager = sessionManager;
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
        await _userAccountService.DeleteById(decryptedId, _sessionManager.UserId ?? 9999);
        return RedirectToPage();
    }
}
