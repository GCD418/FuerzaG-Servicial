using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserAccountService.Domain.Entities;
using UserAccountService.Application.Services;

namespace UserAccountService.Pages.UserAccounts
{
    public class CreateModel : PageModel
    {
        private readonly UserAccountService.Application.Services.UserAccountService _userAccountService;
        private readonly IValidator<UserAccount> _validator;

        public List<string> ValidationErrors { get; set; } = new();

        [BindProperty]
        public UserAccount UserAccount { get; set; } = new();

        public CreateModel(UserAccountService.Application.Services.UserAccountService userAccountService, IValidator<UserAccount> validator)
        {
            _userAccountService = userAccountService;
            _validator = validator;
        }

        public void OnGet() { }

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
                    {
                        ModelState.AddModelError($"UserAccount.{fieldName}", error);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                return Page();
            }

            var isSuccess = await _userAccountService.Create(UserAccount);
            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "No se pudo crear el registro.");
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

            if (errorLower.Contains("carnet") || errorLower.Contains("document"))
                return "DocumentNumber";

            if (errorLower.Contains("rol"))
                return "Role";

            return string.Empty;
        }
    }
}