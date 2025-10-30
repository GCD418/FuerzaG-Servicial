using CommonService.Domain.Entities;
using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// using UserAccountService.Application.Services; // Descomentar

namespace FuerzaGServicial.Pages.UserAccounts
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        // private readonly IPasswordService _passwordService; // Descomentar
        private readonly IValidator<ChangePasswordInput> _validator;
        
        public List<string> ValidationErrors { get; set; } = new();

        [BindProperty] 
        public ChangePasswordInput Input { get; set; } = new();
        
        public ChangePasswordModel(IValidator<ChangePasswordInput> validator)
        {
            _validator = validator;
        }
        
        public IActionResult OnGet()
        {
            return RedirectToPage("/Index");
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();

            var validationResult = _validator.Validate(Input);
            if (validationResult.IsFailure)
            {
                ValidationErrors = validationResult.Errors;
                
                TempData["ValidationErrors"] = ValidationErrors;
                TempData["MustChangePassword"] = true;
                TempData["AllowCloseModal"] = false;
                TempData["ErrorMessage"] = "Por favor corrige los errores:";

                foreach (var error in validationResult.Errors)
                { 
                    var fieldName = MapErrorToField(error);
                        
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        ModelState.AddModelError($"Input.{fieldName}", error);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                
                TempData["MustChangePassword"] = true;
                TempData["ErrorMessage"] = "Por favor corrige los errores del formulario";
                    
                return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
            }
            
            try
            {
                // TODO: Cuando el servicio esté listo, descomentar:
                /*
                var userId = User.FindFirst("userId")?.Value 
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Usuario no autenticado";
                    TempData["MustChangePassword"] = true;
                    return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
                }
                
                await _passwordService.ChangePasswordAsync(userId, Input.CurrentPassword, Input.NewPassword);
                */
                
                // === SIMULACIÓN TEMPORAL - ELIMINAR DESPUÉS ===
                await Task.Delay(500);
                
                // Validación temporal de contraseña actual
                if (Input.CurrentPassword != "temporal123")
                {
                    TempData["ErrorMessage"] = "Contraseña actual incorrecta";
                    TempData["MustChangePassword"] = true;
                    return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
                }
                // === FIN SIMULACIÓN ===
                
                // Éxito
                TempData["SuccessMessage"] = "Contraseña cambiada exitosamente";
                TempData["MustChangePassword"] = false; // Ya no debe mostrar el modal
                
                return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
            }
            catch (InvalidOperationException ex)
            {
                // Error específico de contraseña incorrecta
                TempData["ErrorMessage"] = ex.Message;
                TempData["MustChangePassword"] = true;
                return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar la contraseña: {ex.Message}";
                TempData["MustChangePassword"] = true;
                return RedirectToPage(TempData["ReturnPage"]?.ToString() ?? "/Index");
            }
        }
        
        private string MapErrorToField(string error)
        {
            var errorLower = error.ToLower();
            
            if (errorLower.Contains("contraseña actual") || errorLower.Contains("current password"))
                return "CurrentPassword";
            
            if (errorLower.Contains("nueva contraseña") || errorLower.Contains("new password"))
                return "NewPassword";
            
            if (errorLower.Contains("confirmar") || errorLower.Contains("confirm"))
                return "ConfirmPassword";
            
            return string.Empty;
        }
    }
}