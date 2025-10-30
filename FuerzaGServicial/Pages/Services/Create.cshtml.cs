using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceService.Domain.Entities;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;


namespace FuerzaGServicial.Pages.Services;

[Authorize(Roles = UserRoles.CEO)]
public class Create : PageModel
{
    private readonly ServiceService.Application.Services.ServiceService _serviceService;
    private readonly IValidator<Service> _validator;
    private readonly ISessionManager _sessionManager;

    public List<string> ValidationErrors { get; set; } = [];

    [BindProperty] public Service Service { get; set; } = new();

    public Create(
        ServiceService.Application.Services.ServiceService serviceService,
        IValidator<Service> validator,
        ISessionManager sessionManager)
    {
        _serviceService = serviceService;
        _validator = validator;
        _sessionManager = sessionManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Clear();

        var validation = _validator.Validate(Service);
        if (validation.IsFailure)
        {
            ValidationErrors = validation.Errors;

            foreach (var error in validation.Errors)
            {
                var field = MapErrorToField(error);
                if (!string.IsNullOrEmpty(field))
                    ModelState.AddModelError($"Service.{field}", error);
                else
                    ModelState.AddModelError(string.Empty, error);
            }

            return Page();
        }

        var ok = await _serviceService.Create(Service, _sessionManager.UserId ?? 9999);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "No se pudo crear el registro.");
            return Page();
        }

        return RedirectToPage("/Services/ServicePage");
    }

    private string MapErrorToField(string error)
    {
        var e = error.ToLowerInvariant();
        if (e.Contains("nombre")) return "Name";
        if (e.Contains("tipo")) return "Type";
        if (e.Contains("precio")) return "Price";
        if (e.Contains("descripción") || e.Contains("descripcion")) return "Description";
        return string.Empty;
    }
}
