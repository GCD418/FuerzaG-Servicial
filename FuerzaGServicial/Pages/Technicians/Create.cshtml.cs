using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FuerzaG.Models;
using FuerzaG.Domain.Services.Validations;
using FuerzaG.Application.Services;
using FuerzaG.Domain.Entities;
using FuerzaG.Pages.Shared;
using Microsoft.AspNetCore.Authorization;

namespace FuerzaG.Pages.Technicians
{
    
    [Authorize(Roles = UserRoles.Manager)]
    public class CreateModel : PageModel
    {
        private readonly IValidator<Technician> _validator;
        private readonly TechnicianService _technicianService;

        public List<string> ValidationErrors { get; set; } = new();

        [BindProperty]
        public Technician Form { get; set; } = new();

        public CreateModel(IValidator<Technician> validator, TechnicianService technicianService)
        {
            _validator = validator;
            _technicianService = technicianService;
        }

        public void OnGet()
        { }

        public IActionResult OnPost()
        {
            var validationResult = _validator.Validate(Form);

            if (validationResult.IsFailure)
            {
                ValidationErrors = validationResult.Errors;

                foreach (var error in validationResult.Errors)
                {
                    var sepIndex = error.IndexOf('|');
                    if (sepIndex > 0 && sepIndex < error.Length - 1)
                    {
                        var field = error[..sepIndex].Trim();
                        var message = error[(sepIndex + 1)..].Trim();
                        ModelState.AddModelError($"Form.{field}", message);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            var id = _technicianService.Create(Form);
            if (id <= 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo crear el registro.");
                return Page();
            }

            return RedirectToPage("/Technicians/TechnicianPage");
        }
    }
}
