using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserAccountService.Domain.Entities;
using UserAccountService.Application.Services;

namespace UserAccountService.Pages.UserAccounts
{
    public class EditModel : PageModel
    {
        private readonly UserAccountService.Application.Services.UserAccountService _userAccountService;
        private readonly IValidator<UserAccount> _validator;
        private readonly IDataProtector _protector;

        public List<string> ValidationErrors { get; set; } = new();

        public EditModel(UserAccountService.Application.Services.UserAccountService userAccountService, IValidator<UserAccount> validator, IDataProtectionProvider provider)
        {
            _userAccountService = userAccountService;
            _validator = validator;
            _protector = provider.CreateProtector("UserAccountProtector");
        }

        [BindProperty] public UserAccount UserAccount { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var decryptedId = int.Parse(_protector.Unprotect(id));
            var userAccount = await _userAccountService.GetById(decryptedId);
            if (userAccount is null) return RedirectToPage("/UserAccounts/UserPage");

            UserAccount = userAccount;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();

            var validationResult = _validator.Validate(UserAccount);
            if (validationResult.IsFailure)
            {
                ValidationErrors = validationResult.Errors;

                foreach (var error in validationResult.Errors)
                {
                    var fieldName = MapErrorToField(error);
                    if (!string.IsNullOrEmpty(fieldName))
                        ModelState.AddModelError($"UserAccount.{fieldName}", error);
                    else
                        ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            var isSuccess = await _userAccountService.Update(UserAccount);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el registro.");
                return Page();
            }

            return RedirectToPage("/UserAccounts/UserPage");
        }

        private string MapErrorToField(string error)
        {
            var errorLower = error.ToLower();

            if (errorLower.Contains("apellido paterno"))
                return "FirstLastName";

            if (errorLower.Contains("apellido materno"))
                return "SecondLastName";

            if (errorLower.Contains("nombre") && !errorLower.Contains("apellido"))
                return "Name";

            if (errorLower.Contains("teléfono"))
                return "PhoneNumber";

            if (errorLower.Contains("correo") || errorLower.Contains("email"))
                return "Email";

            if (errorLower.Contains("documento") || errorLower.Contains("ci") ||
                errorLower.Contains("identidad"))
                return "DocumentNumber";

            if (errorLower.Contains("rol"))
                return "Role";

            return string.Empty;
        }
    }
}
