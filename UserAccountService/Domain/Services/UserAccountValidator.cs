using System.Text.RegularExpressions;
using CommonService.Domain.Services.Validations;
using UserAccountService.Domain.Entities;

namespace UserAccountService.Domain.Services;

public class UserAccountValidator : IValidator<UserAccount>
{
    private readonly List<string> _errors = [];

    public Result Validate(UserAccount entity)
    {
        _errors.Clear();
        entity.Name = entity.Name?.Trim() ?? string.Empty;
        entity.FirstLastName = entity.FirstLastName?.Trim() ?? string.Empty;
        entity.SecondLastName = entity.SecondLastName?.Trim();
        entity.Email = entity.Email?.Trim() ?? string.Empty;
        entity.DocumentNumber = entity.DocumentNumber?.Trim() ?? string.Empty;
        
        ValidateName(entity.Name);
        ValidateFirstLastname(entity.FirstLastName);
        ValidateSecondLastname(entity.SecondLastName);
        ValidatePhoneNumber(entity.PhoneNumber);
        ValidateEmail(entity.Email);
        ValidateDocumentNumber(entity.DocumentNumber);
        
        return _errors.Count == 0
            ? Result.Success()
            : Result.Failure(_errors);
    }
        private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _errors.Add("El nombre es requerido");
            return;
        }

        if (name.Length < 3)
        {
            _errors.Add("El nombre debe tener al menos 3 caracteres");
        }

        if (name.Length > 100)
        {
            _errors.Add("El nombre no puede superar los 100 caracteres");
        }

        if (!char.IsLetter(name[0]))
        {
            _errors.Add("El nombre debe comenzar con una letra");
        }

        var prohibitedCharacters = new[] { '<', '>', '/', '\\', '|' };
        if (name.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("El nombre contiene caracteres no permitidos");
        }
    }
    
    private void ValidateFirstLastname(string firstLastname)
    {
        if (string.IsNullOrWhiteSpace(firstLastname))
        {
            _errors.Add("El apellido paterno es requerido");
            return;
        }

        if (firstLastname.Length < 2)
        {
            _errors.Add("El apellido paterno debe tener al menos 2 caracteres");
        }

        if (firstLastname.Length > 100)
        {
            _errors.Add("El apellido paterno no puede superar los 100 caracteres");
        }

        if (!char.IsLetter(firstLastname[0]))
        {
            _errors.Add("El apellido paterno debe comenzar con una letra");
        }

        var prohibitedCharacters = new[] { '<', '>', '/', '\\', '|', '@', '#', '$', '%', '&', '*', '=', '+' };
        if (firstLastname.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("El apellido paterno contiene caracteres no permitidos");
        }
    }

    private void ValidateSecondLastname(string? secondLastname)
    {
        if (string.IsNullOrWhiteSpace(secondLastname))
        {
            return;
        }

        if (secondLastname.Length < 2)
        {
            _errors.Add("El apellido materno debe tener al menos 2 caracteres");
        }

        if (secondLastname.Length > 100)
        {
            _errors.Add("El apellido materno no puede superar los 100 caracteres");
        }

        if (!char.IsLetter(secondLastname[0]))
        {
            _errors.Add("El apellido materno debe comenzar con una letra");
        }

        var prohibitedCharacters = new[] { '<', '>', '/', '\\', '|', '@', '#', '$', '%', '&', '*', '=', '+' };
        if (secondLastname.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("El apellido materno contiene caracteres no permitidos");
        }
    }

    private void ValidatePhoneNumber(int phoneNumber)
    {
        string phoneStr = phoneNumber.ToString();

        if (phoneNumber <= 0)
        {
            _errors.Add("El número de teléfono es requerido");
            return;
        }
        
        if (phoneStr.Length != 8)
        {
            _errors.Add("El número de teléfono debe tener 8 dígitos");
        }
    }

    private void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            _errors.Add("El correo electrónico es requerido");
            return;
        }

        if (email.Length > 255)
        {
            _errors.Add("El correo electrónico no puede superar los 255 caracteres");
        }
        
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailRegex))
        {
            _errors.Add("El formato del correo electrónico no es válido");
        }
        
        var prohibitedCharacters = new[] { '<', '>', '/', '\\', '|', ' ' };
        if (email.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("El correo electrónico contiene caracteres no permitidos");
        }
    }

    private void ValidateDocumentNumber(string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            _errors.Add("El carnet de identidad es requerido");
            return;
        }
        
        string documentNumberClean = documentNumber.Replace(" ", "").Replace("-", "");

        if (documentNumberClean.Length < 6)
        {
            _errors.Add("El carnet de identidad debe tener al menos 6 caracteres");
        }

        if (documentNumberClean.Length > 14)
        {
            _errors.Add("El carnet de identidad no puede superar los 14 caracteres");
        }
        
        if (!Regex.IsMatch(documentNumber, @"^[0-9\s\-]+$"))
        {
            _errors.Add("El carnet de identidad solo puede contener números");
        }
        
        if (!documentNumberClean.Any(char.IsDigit))
        {
            _errors.Add("El carnet de identidad debe contener al menos un número");
        }
    }
}