using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwnerService.Domain.Entities;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.Owners;

[Authorize(Roles = UserRoles.Manager)]
public class EditModel : PageModel
{
    private readonly OwnerService.Application.Services.OwnerService  _ownerService;
    private readonly IValidator<Owner> _validator;
    private readonly IDataProtector _protector;
    private readonly ISessionManager _sessionManager;
    
    public List<string> ValidationErrors { get; set; } = [];

    public EditModel(OwnerService.Application.Services.OwnerService ownerService, 
        IValidator<Owner> validator, 
        IDataProtectionProvider provider,
        ISessionManager sessionManager)
    {
        _ownerService = ownerService;
        _validator = validator;
        _protector = provider.CreateProtector("OwnerProtector");
        _sessionManager = sessionManager;
    }

    
    [BindProperty] public Owner Owner { get; set; } = new();
    public async Task<IActionResult> OnGetAsync(string id)
    {
        var decryptedId = int.Parse(_protector.Unprotect(id));
        var owner = await _ownerService.GetById(decryptedId);
        if (owner is null) return RedirectToPage("/Owners/OwnerPage");

        Owner = owner;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Clear();

        var validationResult = _validator.Validate(Owner);
        if (validationResult.IsFailure)
        {
            ValidationErrors = validationResult.Errors;

            foreach (var error in validationResult.Errors)
            {
                var fieldName = MapErrorToField(error);
                if (!string.IsNullOrEmpty(fieldName))
                    ModelState.AddModelError($"Owner.{fieldName}", error);
                else
                    ModelState.AddModelError(string.Empty, error);
            }

            return Page();
        }

        var isSuccess = await _ownerService.Update(Owner, _sessionManager.UserId ?? 9999);

        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el registro.");
            return Page();
        }

        return RedirectToPage("/Owners/OwnerPage");
    }
    
    private string MapErrorToField(string error)
    {
        var errorLower = error.ToLower();

        if (errorLower.Contains("apellido paterno"))
            return "FirstLastname";

        if (errorLower.Contains("apellido materno"))
            return "SecondLastname";

        if (errorLower.Contains("nombre") && !errorLower.Contains("apellido"))
            return "Name";

        if (errorLower.Contains("teléfono"))
            return "PhoneNumber";

        if (errorLower.Contains("correo") || errorLower.Contains("email"))
            return "Email";

        if (errorLower.Contains("carnet") || errorLower.Contains(" ci ") ||
            errorLower.Contains("identidad"))
            return "Ci";

        if (errorLower.Contains("dirección"))
            return "Address";
        
        if (errorLower.Contains("complemento"))
            return "Complement";

        return string.Empty;
    }
}