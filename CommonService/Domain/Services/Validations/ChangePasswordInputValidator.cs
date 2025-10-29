using CommonService.Domain.Entities;

namespace CommonService.Domain.Services.Validations;

public class ChangePasswordInputValidator : IValidator<ChangePasswordInput>
{
    public Result Validate(ChangePasswordInput entity)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(entity.CurrentPassword))
        {
            errors.Add("La contraseña actual es requerida");
        }

        if (string.IsNullOrWhiteSpace(entity.NewPassword))
        {
            errors.Add("La nueva contraseña es requerida");
        }
        else
        {
            if (entity.NewPassword.Length < 8)
            {
                errors.Add("La nueva contraseña debe tener al menos 8 caracteres");
            }

            if (!HasUpperCase(entity.NewPassword))
            {
                errors.Add("La nueva contraseña debe contener al menos una letra mayúscula");
            }

            if (!HasLowerCase(entity.NewPassword))
            {
                errors.Add("La nueva contraseña debe contener al menos una letra minúscula");
            }

            if (!HasDigit(entity.NewPassword))
            {
                errors.Add("La nueva contraseña debe contener al menos un número");
            }

            if (!string.IsNullOrWhiteSpace(entity.CurrentPassword) && 
                entity.NewPassword == entity.CurrentPassword)
            {
                errors.Add("La nueva contraseña debe ser diferente a la actual");
            }
        }

        if (string.IsNullOrWhiteSpace(entity.ConfirmPassword))
        {
            errors.Add("Debes confirmar la nueva contraseña");
        }
        else if (entity.NewPassword != entity.ConfirmPassword)
        {
            errors.Add("La confirmación de contraseña no coincide con la nueva contraseña");
        }

        return errors.Count == 0 
            ? Result.Success() 
            : Result.Failure(errors);
    }

    private bool HasUpperCase(string password)
    {
        return password.Any(char.IsUpper);
    }

    private bool HasLowerCase(string password)
    {
        return password.Any(char.IsLower);
    }

    private bool HasDigit(string password)
    {
        return password.Any(char.IsDigit);
    }
}