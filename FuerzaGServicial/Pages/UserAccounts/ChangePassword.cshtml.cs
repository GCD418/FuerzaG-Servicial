using CommonService.Domain.Entities;
using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FuerzaGServicial.Pages.UserAccounts
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly UserAccountService.Application.Services.UserAccountService _userAccountService;
        private readonly IValidator<ChangePasswordInput> _validator;

        [BindProperty]
        public ChangePasswordInput Input { get; set; } = new();
        
        public List<string> validationErrors { get; set; } = new();
        public string errorMessage { get; set; }
        public string successMessage { get; set; }
        public bool mustShowModal { get; set; } = false;
        public bool allowCloseModal { get; set; } = true;

        public ChangePasswordModel(
            UserAccountService.Application.Services.UserAccountService userAccountService,
            IValidator<ChangePasswordInput> validator)
        {
            _userAccountService = userAccountService;
            _validator = validator;
        }

        public void OnGet(bool showModal = false, bool mustChange = false)
        {
            mustShowModal = showModal || mustChange;
            allowCloseModal = !mustChange;
            ViewData["ShowChangePasswordModal"] = showModal;
            ViewData["SuccessMessage"] = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Entró a OnPostAsync");

            var validationResult = _validator.Validate(Input);
            if (validationResult.IsFailure)
            {
                validationErrors = validationResult.Errors;
                mustShowModal = true;
                allowCloseModal = true;
                return Page();
            }

            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    errorMessage = "Usuario no autenticado.";
                    mustShowModal = true;
                    allowCloseModal = true;
                    return Page();
                }

                var currentUser = await _userAccountService.GetById(userId);
                if (currentUser == null)
                {
                    errorMessage = "Usuario no encontrado.";
                    mustShowModal = true;
                    allowCloseModal = true;
                    return Page();
                }

                if (!VerifyPassword(Input.CurrentPassword, currentUser.Password))
                {
                    errorMessage = "La contraseña actual es incorrecta.";
                    mustShowModal = true;
                    allowCloseModal = true;
                    return Page();
                }

                var hashedNewPassword = HashPassword(Input.NewPassword);
                var success = await _userAccountService.ChangePassword(userId, hashedNewPassword);

                if (!success)
                {
                    errorMessage = "No se pudo cambiar la contraseña. Intenta nuevamente.";
                    mustShowModal = true;
                    allowCloseModal = true;
                    return Page();
                }
                
                var isFirstLoginClaim = User.FindFirst("IsFirstLogin")?.Value;
                if (isFirstLoginClaim == "True")
                {
                    await _userAccountService.UpdateIsFirstLoginAsync(userId, false);
                    // Actualizar el claim en la sesión actual
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                    var claims = User.Claims.Where(c => c.Type != "IsFirstLogin").ToList();
                    claims.Add(new System.Security.Claims.Claim("IsFirstLogin", "False"));
                    
                    var newIdentity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                        new System.Security.Claims.ClaimsPrincipal(newIdentity));
                    
                    // Redirigir al index después de cambio obligatorio
                    return RedirectToPage("/Index");
                }

                successMessage = "¡Contraseña cambiada exitosamente!";
                mustShowModal = true;
                allowCloseModal = true;
                ViewData["ShowChangePasswordModal"] = true;
                ViewData["AutoCloseModal"] = true;

                return Page();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al cambiar la contraseña: {ex.Message}";
                mustShowModal = true;
                allowCloseModal = true;
                return Page();
            }
        }
        
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }
    }
}
