using TechnicianService.Application;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace FuerzaG.Pages.Technicians
{
    [Authorize(Roles = "Manager")]
    public class DeleteModel : PageModel
    {
        private readonly TechnicianService.Application.Services.TechnicianService _technicianService;
        private readonly IDataProtector _protector;

        public DeleteModel(TechnicianService.Application.Services.TechnicianService technicianService, IDataProtectionProvider provider)
        {
            _technicianService = technicianService;
            _protector = provider.CreateProtector("TechnicianProtector");
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Id inválido.");
            }

            int decryptedId;
            try
            {
                decryptedId = int.Parse(_protector.Unprotect(id));
            }
            catch
            {
                return BadRequest("Id no válido o manipulado.");
            }

            await _technicianService.DeleteById(decryptedId);
            return RedirectToPage("/Technicians/TechnicianPage");
        }
    }
}
