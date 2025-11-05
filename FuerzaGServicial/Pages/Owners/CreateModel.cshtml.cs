using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwnerService.Domain.Entities;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.Owners;


[Authorize(Roles = UserRoles.Manager)]
public class CreateModel : PageModel
{
    private readonly OwnerService.Application.Services.OwnerService  _ownerService;
    private readonly IValidator<Owner> _validator;
    private readonly ISessionManager _sessionManager;
    public List<string> ValidationErrors { get; set; } = [];

    [BindProperty] public Owner Owner { get; set; } = new();

    public CreateModel(OwnerService.Application.Services.OwnerService ownerService, 
        IValidator<Owner> validator,
        ISessionManager sessionManager)
    {
        _ownerService = ownerService;
        _validator = validator;
        _sessionManager = sessionManager;
    }
    
    public void OnGet()
    { }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Clear();
        // if (!ModelState.IsValid) return Page();
        var validationResult = _validator.Validate(Owner);
        if (validationResult.IsFailure)
        {
            ValidationErrors = validationResult.Errors;

            foreach (var error in validationResult.Errors)
            { 
                var fieldName = MapErrorToField(error);
                    
                if (!string.IsNullOrEmpty(fieldName))
                {
                    ModelState.AddModelError($"Owner.{fieldName}", error);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            return Page();
        }

        var isSuccess = await _ownerService.Create(Owner, _sessionManager.UserId ?? 9999);
        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, "No se pudo crear el registro.");
            return Page();
        }
        return RedirectToPage("/Owners/OwnerPage");
    }
    
    private string MapErrorToField(string error)
    {
        var errorLower = error.ToLower();
        
        // Orden importante: primero los más específicos
        if (errorLower.Contains("apellido paterno"))
            return "FirstLastname";
        
        if (errorLower.Contains("apellido materno"))
            return "SecondLastname";
        
        // Después el nombre (para evitar conflicto con "apellido")
        if (errorLower.Contains("nombre") && !errorLower.Contains("apellido"))
            return "Name";
        
        if (errorLower.Contains("teléfono"))
            return "PhoneNumber";
        
        if (errorLower.Contains("correo") || errorLower.Contains("email"))
            return "Email";
        
        // Carnet de identidad tiene varias formas
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