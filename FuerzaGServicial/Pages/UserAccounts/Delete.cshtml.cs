using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserAccountService.Domain.Entities;

namespace FuerzaGServicial.Pages.UserAccounts;
[Authorize(Roles = UserRoles.CEO)]
 public class DeleteModel : PageModel
 {
        private readonly UserAccountService.Application.Services.UserAccountService _userAccountService;
        private readonly IDataProtector _protector;

        public DeleteModel(UserAccountService.Application.Services.UserAccountService userAccountService, IDataProtectionProvider provider)
        {
            _userAccountService = userAccountService;
            _protector = provider.CreateProtector("UserAccountProtector");
        }

        public void OnGet()
        { }

        public async Task<IActionResult> OnPost(string id)
        {
            var decryptedId = int.Parse(_protector.Unprotect(id));
            await _userAccountService.DeleteById(decryptedId);
            return RedirectToPage("/UserAccounts/UserPage");
        }
 }
