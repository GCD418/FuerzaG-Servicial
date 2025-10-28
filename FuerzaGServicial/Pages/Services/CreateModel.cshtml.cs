using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceService.Application.Services;
using ServiceService.Domain.Entities;

namespace FuerzaGServicial.Pages.Services;

[Authorize(Roles = "Manager")]
public class CreateModel : PageModel
{
    private readonly ServiceService.Application.Services.ServiceService _serviceService;
    private readonly IValidator<Service> _validator;

    public List<string> ValidationErrors { get; set; } = [];

    [BindProperty] public Service Service { get; set; } = new();

    public CreateModel(
        ServiceService.Application.Services.ServiceService serviceService,
        IValidator<Service> validator)
    {
        _serviceService = serviceService;
        _validator = validator;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        // evitamos errores de DataAnnotations en paralelo al validador
        ModelState.Clear();

        // (opcional) si llega como texto y deseas normalizar precio:
        // if (Request.Form.TryGetValue("Service.Price", out var raw) &&
        //     decimal.TryParse(raw, out var parsed)) Service.Price = parsed;

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

        var ok = await _serviceService.Create(Service);
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
