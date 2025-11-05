using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.UserAccounts;

[Authorize(Roles = UserRoles.CEO)]
public class EditModel : PageModel
{
    private readonly UserAccountService.Application.Services.UserAccountService _userAccountService;
    private readonly IValidator<UserAccount> _validator;
    private readonly IDataProtector _protector;
    private readonly ISessionManager _sessionManager;

    [BindProperty]
    public UserAccount UserAccount { get; set; } = new();

    public List<string> ValidationErrors { get; set; } = new();

    public EditModel(
        UserAccountService.Application.Services.UserAccountService userAccountService,
        IValidator<UserAccount> validator,
        IDataProtectionProvider provider,
        ISessionManager sessionManager)
    {
        _userAccountService = userAccountService;
        _validator = validator;
        _protector = provider.CreateProtector("UserAccountProtector");
        _sessionManager = sessionManager;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToPage("/UserAccounts/UserPage");

        if (!int.TryParse(_protector.Unprotect(id), out var decryptedId))
            return RedirectToPage("/UserAccounts/UserPage");

        var user = await _userAccountService.GetById(decryptedId);
        if (user == null)
            return RedirectToPage("/UserAccounts/UserPage");

        UserAccount = user;
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

        // Mantener UserName existente
        var existingUser = await _userAccountService.GetById(UserAccount.Id);
        if (existingUser != null)
            UserAccount.UserName = existingUser.UserName;

        var isSuccess = await _userAccountService.Update(UserAccount, _sessionManager.UserId ?? 9999);
        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el usuario.");
            return Page();
        }

        TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
        return RedirectToPage("/UserAccounts/UserPage");
    }

    private string MapErrorToField(string error)
    {
        var errorLower = error.ToLowerInvariant();

        if (errorLower.Contains("nombre") && !errorLower.Contains("apellido")) return "Name";
        if (errorLower.Contains("primer apellido")) return "FirstLastName";
        if (errorLower.Contains("segundo apellido")) return "SecondLastName";
        if (errorLower.Contains("correo") || errorLower.Contains("email")) return "Email";
        if (errorLower.Contains("teléfono") || errorLower.Contains("telefono")) return "PhoneNumber";
        if (errorLower.Contains("documento") || errorLower.Contains("ci") || errorLower.Contains("carnet")) return "DocumentNumber";
        if (errorLower.Contains("rol")) return "Role";
        if (errorLower.Contains("complemento")) return "DocumentComplement";

        return string.Empty; // errores generales
    }
}
