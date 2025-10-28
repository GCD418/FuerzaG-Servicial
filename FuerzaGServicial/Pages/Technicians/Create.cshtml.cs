using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CommonService.Domain.Services.Validations;
using TechnicianService.Application;                 // <- CORRECTO
using TechnicianService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;


namespace FuerzaG.Pages.Technicians
{
    [Authorize(Roles = "Manager")]
    public class CreateModel : PageModel
    {
        private readonly IValidator<Technician> _validator;
        private readonly Service _technicianService; // <- clase real

        public List<string> ValidationErrors { get; set; } = new();

        [BindProperty]
        public Technician Form { get; set; } = new();

        public CreateModel(IValidator<Technician> validator, Service technicianService)
        {
            _validator = validator;
            _technicianService = technicianService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var validationResult = _validator.Validate(Form);
            if (validationResult.IsFailure)
            {
                ValidationErrors = validationResult.Errors;
                foreach (var error in validationResult.Errors)
                {
                    var i = error.IndexOf('|');
                    if (i > 0 && i < error.Length - 1)
                    {
                        var field = error[..i].Trim();
                        var msg = error[(i + 1)..].Trim();
                        ModelState.AddModelError($"Form.{field}", msg);
                    }
                    else ModelState.AddModelError(string.Empty, error);
                }
                return Page();
            }

            if (!ModelState.IsValid) return Page();

            var ok = await _technicianService.Create(Form);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "No se pudo crear el registro.");
                return Page();
            }
            return RedirectToPage("/Technicians/TechnicianPage");
        }
    }
}
