using CommonService.Domain.Entities;
using CommonService.Domain.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BCrypt.Net;

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

        public void OnGet(bool showModal = false)
        {
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
                
                successMessage = "¡Contraseña cambiada exitosamente!";
                mustShowModal = true;
                allowCloseModal = true;

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
