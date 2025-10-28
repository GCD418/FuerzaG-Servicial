using TechnicianService.Application;
using TechnicianService.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace FuerzaG.Pages.Technicians
{
    [Authorize(Roles = "Manager")]
    public class EditModel : PageModel
    {
        private readonly Service _technicianService;
        private readonly IDataProtector _protector;

        public EditModel(Service technicianService, IDataProtectionProvider provider)
        {
            _technicianService = technicianService;
            _protector = provider.CreateProtector("TechnicianProtector");
        }

        [BindProperty] public string EncryptedId { get; set; } = string.Empty;
        [BindProperty] public Technician Form { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var decryptedId = int.Parse(_protector.Unprotect(id));
            var entity = await _technicianService.GetById(decryptedId);
            if (entity is null) return RedirectToPage("/Technicians/TechnicianPage");
            EncryptedId = id;
            Form = entity;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var ok = await _technicianService.Update(Form);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar.");
                return Page();
            }
            return RedirectToPage("/Technicians/TechnicianPage");
        }
    }
}
