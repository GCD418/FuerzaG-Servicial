using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceService.Domain.Entities;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Pages.Services;

[Authorize(Roles = UserRoles.CEO)]
public class Edit : PageModel
{
    private readonly ServiceService.Application.Services.ServiceService _serviceService;
    private readonly IValidator<Service> _validator;
    private readonly IDataProtector _protector;
    private readonly ISessionManager _sessionManager;

    public List<string> ValidationErrors { get; set; } = [];

    public Edit(
        ServiceService.Application.Services.ServiceService serviceService,
        IValidator<Service> validator,
        IDataProtectionProvider provider,
        ISessionManager sessionManager)
    {
        _serviceService = serviceService;
        _validator = validator;
        _protector = provider.CreateProtector("ServiceProtector");
        _sessionManager = sessionManager;
    }

    [BindProperty] public Service Service { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var decryptedId = int.Parse(_protector.Unprotect(id));
        var service = await _serviceService.GetById(decryptedId);
        if (service is null) return RedirectToPage("/Services/ServicePage");

        Service = service;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Clear();

        var validationResult = _validator.Validate(Service);
        if (validationResult.IsFailure)
        {
            ValidationErrors = validationResult.Errors;

            foreach (var error in validationResult.Errors)
            {
                var fieldName = MapErrorToField(error);
                if (!string.IsNullOrEmpty(fieldName))
                    ModelState.AddModelError($"Service.{fieldName}", error);
                else
                    ModelState.AddModelError(string.Empty, error);
            }

            return Page();
        }

        var ok = await _serviceService.Update(Service, _sessionManager.UserId ?? 9999);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el servicio.");
            return Page();
        }

        return RedirectToPage("/Services/ServicePage");
    }

    private string MapErrorToField(string error)
    {
        var e = error.ToLower();

        if (e.Contains("nombre")) return "Name";
        if (e.Contains("tipo")) return "Type";
        if (e.Contains("precio")) return "Price";
        if (e.Contains("descripción") || e.Contains("descripcion")) return "Description";

        return string.Empty;
    }
}
