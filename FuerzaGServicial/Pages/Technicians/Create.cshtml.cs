using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechnicianService.Domain.Entities;
using TechnicianService.Domain.Services;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.Technicians;

[Authorize(Roles = UserRoles.Manager)]
public class CreateModel : PageModel
{
    private readonly IValidator<Technician> _validator;
    private readonly TechnicianService.Application.Services.TechnicianService _technicianService;
    private readonly ISessionManager _sessionManager;

    [BindProperty]
    public Technician Form { get; set; } = new();

    public List<string> ValidationErrors { get; set; } = new();

    public CreateModel(
        IValidator<Technician> validator,
        TechnicianService.Application.Services.TechnicianService technicianService,
        ISessionManager sessionManager)
    {
        _validator = validator;
        _technicianService = technicianService;
        _sessionManager = sessionManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Clear();

        var validationResult = _validator.Validate(Form);

        if (validationResult.IsFailure)
        {
            ValidationErrors = validationResult.Errors;

            foreach (var error in validationResult.Errors)
            {
                string fieldName = MapErrorToField(error);
                if (!string.IsNullOrEmpty(fieldName))
                    ModelState.AddModelError($"Form.{fieldName}", error);
                else
                    ModelState.AddModelError(string.Empty, error);
            }

            return Page();
        }

        var ok = await _technicianService.Create(Form, _sessionManager.UserId ?? 9999);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "No se pudo crear el registro.");
            return Page();
        }

        return RedirectToPage("/Technicians/TechnicianPage");
    }

    private string MapErrorToField(string error)
    {
        var lower = error.ToLower();

        if (lower.Contains("nombre")) return "Name";
        if (lower.Contains("primer apellido")) return "FirstLastName";
        if (lower.Contains("segundo apellido")) return "SecondLastName";
        if (lower.Contains("tel�fono") || lower.Contains("telefono")) return "PhoneNumber";
        if (lower.Contains("email")) return "Email";
        if (lower.Contains("documento") || lower.Contains("ci")) return "DocumentNumber";
        if (lower.Contains("complemeto")) return "Comeplement";
        if (lower.Contains("dirección") || lower.Contains("direccion")) return "Address";
        if (lower.Contains("salario")) return "BaseSalary";

        return string.Empty;
    }
}
