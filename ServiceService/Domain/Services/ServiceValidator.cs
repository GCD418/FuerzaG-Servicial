using System.Globalization;
using System.Text.RegularExpressions;
using FuerzaG.Domain.Entities;

namespace FuerzaG.Domain.Services.Validations;

public class ServiceValidator : IValidator<Service>
{
    private readonly List<string> _errors = new();

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
            _errors.Add("El nombre del servicio es obligatorio");
            return;
        }

        if (name.Length < 3)
            _errors.Add("El nombre debe tener al menos 3 caracteres");

        if (name.Length > 100)
            _errors.Add("El nombre no puede superar los 100 caracteres");

        if (!Regex.IsMatch(name, @"^[\p{L}\d\s]+$"))
            _errors.Add("El nombre solo puede contener letras, números y espacios");
    }

    private void ValidateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            _errors.Add("El tipo de servicio es obligatorio");
            return;
        }

        if (type.Length < 3)
            _errors.Add("El tipo debe tener al menos 3 caracteres");

        if (type.Length > 50)
            _errors.Add("El tipo no puede superar los 50 caracteres");
    }

    private void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            _errors.Add("El precio debe ser mayor que cero");
            return;
        }

        if (decimal.Round(price, 2) != price)
            _errors.Add("El precio solo puede tener hasta dos decimales");
    }

    public void ValidatePriceFormat(string rawPrice)
    {
        if (string.IsNullOrWhiteSpace(rawPrice))
        {
            _errors.Add("El campo Precio es obligatorio");
            return;
        }

        rawPrice = rawPrice.Trim();

        string normalized = rawPrice
            .Replace(" ", "")
            .Replace(",", "") 
            .Replace('.', '.'); 

        if (Regex.IsMatch(rawPrice, @"\d+(\.\d{3})*,\d{1,2}$"))
            normalized = rawPrice.Replace(".", "").Replace(",", ".");

        if (!decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsed))
        {
            _errors.Add($"El valor '{rawPrice}' no es válido para Precio. Use el formato 1,200.50");
            return;
        }

        if (parsed <= 0)
            _errors.Add("El precio debe ser mayor que cero");

        if (decimal.Round(parsed, 2) != parsed)
            _errors.Add("El precio solo puede tener hasta dos decimales");
    }


    private void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            _errors.Add("La descripción es obligatoria");
            return;
        }

        if (description.Length < 3)
            _errors.Add("La descripción debe tener al menos 3 caracteres");

        if (description.Length > 500)
            _errors.Add("La descripción no puede superar los 500 caracteres");
    }
}
