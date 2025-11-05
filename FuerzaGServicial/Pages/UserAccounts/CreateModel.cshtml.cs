using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserAccountService.Application.Facades;
using UserAccountService.Domain.Entities;

namespace FuerzaGServicial.Pages.UserAccounts;

[Authorize(Roles = UserRoles.CEO)]
public class CreateModel : PageModel
{
    private readonly SessionFacade _sessionFacade;
    private readonly IValidator<UserAccount> _validator;

    public List<string> ValidationErrors { get; set; } = new();

    [BindProperty]
    public UserAccount UserAccount { get; set; } = new();

    public CreateModel(
        SessionFacade sessionFacade,
        IValidator<UserAccount> validator)
    {
        _sessionFacade = sessionFacade;
        _validator = validator;
    }

    public void OnGet()
    {
        if (string.IsNullOrEmpty(UserAccount.Role))
            UserAccount.Role = "Manager";
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

        var isSuccess = await _sessionFacade.CreateUserAccount(UserAccount);
        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, "No se pudo crear el usuario.");
            return Page();
        }

        return RedirectToPage("/UserAccounts/UserPage");
    }

    private string MapErrorToField(string error)
    {
        var errorLower = error.ToLower();

        if (errorLower.Contains("primer apellido"))
            return "FirstLastName";

        if (errorLower.Contains("segundo apellido"))
            return "SecondLastName";

        if (errorLower.Contains("nombre") && !errorLower.Contains("apellido"))
            return "Name";

        if (errorLower.Contains("teléfono") || errorLower.Contains("telefono"))
            return "PhoneNumber";

        if (errorLower.Contains("correo") || errorLower.Contains("email"))
            return "Email";

        if (errorLower.Contains("documento") || errorLower.Contains("ci") || errorLower.Contains("carnet"))
            return "DocumentNumber";

        if (errorLower.Contains("rol"))
            return "Role";
        
        if (errorLower.Contains("complemento"))
            return "DocumentComplement";

        return string.Empty;
    }
}
