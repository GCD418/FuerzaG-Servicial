using System.Text.RegularExpressions;
using CommonService.Domain.Services.Validations;
using OwnerService.Domain.Entities;

namespace OwnerService.Domain.Services;

public class OwnerValidator : IValidator<Owner>
{
    private readonly List<string> _errors = [];

    public Result Validate(Owner entity)
    {
        _errors.Clear();
        entity.Name = entity.Name?.Trim() ?? string.Empty;
        entity.FirstLastname = entity.FirstLastname?.Trim() ?? string.Empty;
        entity.SecondLastname = entity.SecondLastname?.Trim();
        entity.Email = entity.Email?.Trim() ?? string.Empty;
        entity.Ci = entity.Ci?.Trim() ?? string.Empty;
        entity.Address = entity.Address?.Trim() ?? string.Empty;
        
        ValidateName(entity.Name);
        ValidateFirstLastname(entity.FirstLastname);
        ValidateSecondLastname(entity.SecondLastname);
        ValidatePhoneNumber(entity.PhoneNumber);
        ValidateEmail(entity.Email);
        ValidateCi(entity.Ci);
        ValidateAddress(entity.Address);
        
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

    private void ValidateCi(string ci)
    {
        if (string.IsNullOrWhiteSpace(ci))
        {
            _errors.Add("El carnet de identidad es requerido");
            return;
        }
        
        string ciClean = ci.Replace(" ", "").Replace("-", "");

        if (ciClean.Length < 6)
        {
            _errors.Add("El carnet de identidad debe tener al menos 6 caracteres");
        }

        if (ciClean.Length > 9)
        {
            _errors.Add("El carnet de identidad no puede superar los 9 caracteres");
        }
        
        if (!Regex.IsMatch(ci, @"^[0-9\s\-]+$"))
        {
            _errors.Add("El carnet de identidad solo puede contener números");
        }
        
        if (!ciClean.Any(char.IsDigit))
        {
            _errors.Add("El carnet de identidad debe contener al menos un número");
        }
    }

    private void ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            _errors.Add("La dirección es requerida");
            return;
        }

        if (address.Length < 5)
        {
            _errors.Add("La dirección debe tener al menos 5 caracteres");
        }

        if (address.Length > 100)
        {
            _errors.Add("La dirección no puede superar los 100 caracteres");
        }
        
        var prohibitedCharacters = new[] { '<', '>', '|', '\\' };
        if (address.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("La dirección contiene caracteres no permitidos");
        }
    }
}