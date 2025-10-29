using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuerzaGServicial.Pages.UserAccounts
{
    public class ChangePasswordModel : PageModel
    {

        
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required(ErrorMessage = "La contraseña actual es requerida")]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }
            
            [Required(ErrorMessage = "La nueva contraseña es requerida")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }
            
            [Required(ErrorMessage = "Debes confirmar la nueva contraseña")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
            public string ConfirmPassword { get; set; }
        }
        
        public IActionResult OnGet()
        {
            // Redirigir a la página de origen si acceden directamente
            return RedirectToPage("/Index");
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor corrige los errores del formulario";
                return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
            }
            
            try
            {
                // TODO: Cuando el servicio esté listo:
                // await _passwordService.ChangePasswordAsync(userId, Input.CurrentPassword, Input.NewPassword);
                
                // SIMULACIÓN TEMPORAL
                await Task.Delay(500);
                
                if (Input.CurrentPassword != "temporal123")
                {
                    TempData["ErrorMessage"] = "Contraseña actual incorrecta";
                    TempData["MustChangePassword"] = true;
                    return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
                }
                
                // Éxito
                TempData["SuccessMessage"] = "Contraseña cambiada exitosamente (MODO PRUEBA)";
                TempData["MustChangePassword"] = false; // Ya no debe mostrar el modal
                
                return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                TempData["MustChangePassword"] = true;
                return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
            }
        }
    }
}