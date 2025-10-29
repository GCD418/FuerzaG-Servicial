using System.Text.RegularExpressions;
using CommonService.Domain.Services.Validations;
using ServiceService.Domain.Entities;

namespace ServiceService.Domain.Services; 

public class ServiceValidator : IValidator<Service>
{
    private readonly List<string> _errors = [];

    public Result Validate(Service entity)
    {
        _errors.Clear();

        entity.Name = entity.Name?.Trim() ?? string.Empty;
        entity.Type = entity.Type?.Trim() ?? string.Empty;
        entity.Description = entity.Description?.Trim() ?? string.Empty;

        ValidateName(entity.Name);
        ValidateType(entity.Type);
        ValidatePrice(entity.Price);
        ValidateDescription(entity.Description);

        return _errors.Count == 0
            ? Result.Success()
            : Result.Failure(_errors);
    }


    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _errors.Add("El nombre del servicio es requerido");
            return;
        }

        if (name.Length < 3)
        {
            _errors.Add("El nombre del servicio debe tener al menos 3 caracteres");
        }

        if (name.Length > 100)
        {
            _errors.Add("El nombre del servicio no puede superar los 100 caracteres");
        }

        if (!char.IsLetter(name[0]))
        {
            _errors.Add("El nombre del servicio debe comenzar con una letra");
        }

        var prohibitedCharacters = new[] { '<', '>', '/', '\\', '|', '@', '#', '$', '%', '&', '*', '=', '+' };
        if (name.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("El nombre del servicio contiene caracteres no permitidos");
        }
    }


    private void ValidateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            _errors.Add("El tipo de servicio es requerido");
            return;
        }

        if (type.Length < 3)
        {
            _errors.Add("El tipo de servicio debe tener al menos 3 caracteres");
        }

        if (type.Length > 50)
        {
            _errors.Add("El tipo de servicio no puede superar los 50 caracteres");
        }

        if (!char.IsLetter(type[0]))
        {
            _errors.Add("El tipo de servicio debe comenzar con una letra");
        }

        var prohibitedCharacters = new[] { '<', '>', '/', '\\', '|', '@', '#', '$', '%', '&', '*', '=', '+' };
        if (type.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("El tipo de servicio contiene caracteres no permitidos");
        }
    }


    private void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            _errors.Add("El precio debe ser mayor que cero");
            return;
        }

        if (decimal.Round(price, 2) != price)
        {
            _errors.Add("El precio solo puede tener hasta dos decimales");
        }
    }


    private void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            _errors.Add("La descripción es requerida");
            return;
        }

        if (description.Length < 5)
        {
            _errors.Add("La descripción debe tener al menos 5 caracteres");
        }

        if (description.Length > 500)
        {
            _errors.Add("La descripción no puede superar los 500 caracteres");
        }

        var prohibitedCharacters = new[] { '<', '>', '/', '\\', '|', '@', '#', '$', '%' };
        if (description.Any(c => prohibitedCharacters.Contains(c)))
        {
            _errors.Add("La descripción contiene caracteres no permitidos");
        }
    }
}
