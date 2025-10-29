using System.Collections.Generic;
using TechnicianService.Application.Services;
using TechnicianService.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CommonService.Domain.Services.Validations;
using UserAccountService.Domain.Entities;

namespace FuerzaGServicial.Pages.Technicians;

[Authorize(Roles = UserRoles.Manager)]
public class TechnicianPage : PageModel
{
    public List<Technician> Technicians { get; set; } = new();
    private readonly TechnicianService.Application.Services.TechnicianService _technicianService;
    private readonly IDataProtector _protector;

    public TechnicianPage(TechnicianService.Application.Services.TechnicianService technicianService, IDataProtectionProvider provider)
    {
        _technicianService = technicianService;
        _protector = provider.CreateProtector("TechnicianProtector");
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Technicians = (await _technicianService.GetAll()).ToList();
        return Page();
    }

    public string EncryptId(int id) => _protector.Protect(id.ToString());

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var decryptedId = int.Parse(_protector.Unprotect(id));
        await _technicianService.DeleteById(decryptedId);
        return RedirectToPage("/Technicians/TechnicianPage");
    }
}
