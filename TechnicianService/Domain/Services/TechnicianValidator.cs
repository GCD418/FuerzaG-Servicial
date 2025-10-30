using CommonService.Domain.Services.Validations;
using TechnicianService.Domain.Entities;

namespace TechnicianService.Domain.Services
{
    public class TechnicianValidator : IValidator<Technician>
    {
        private readonly List<string> _errors = [];

        public Result Validate(Technician entity)
        {
            _errors.Clear();

            ValidateName(entity.Name);
            ValidateFirstLastName(entity.FirstLastName);
            ValidateSecondLastName(entity.SecondLastName);
            ValidatePhoneNumber(entity.PhoneNumber);
            ValidateEmail(entity.Email);
            ValidateDocumentNumber(entity.DocumentNumber);
            ValidateAddress(entity.Address);
            ValidateBaseSalary(entity.BaseSalary);

            return _errors.Count == 0
                ? Result.Success()
                : Result.Failure(_errors);
        }

        private static readonly char[] ProhibitedChars = new[] { '<', '>', '/', '\\', '|' };

        private void ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _errors.Add("El nombre es requerido");
                return;
            }
            if (name.Length < 3) _errors.Add("El nombre debe tener al menos 3 caracteres");
            if (name.Length > 100) _errors.Add("El nombre no puede superar los 100 caracteres");
            if (!char.IsLetter(name[0])) _errors.Add("El nombre debe comenzar con una letra");
            if (name.Any(c => ProhibitedChars.Contains(c))) _errors.Add("El nombre contiene caracteres no permitidos");
        }

        private void ValidateFirstLastName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _errors.Add("El primer apellido es requerido");
                return;
            }
            if (value.Length < 2) _errors.Add("Debe tener al menos 2 caracteres");
            if (value.Length > 100) _errors.Add("No puede superar los 100 caracteres");
            if (value.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidateSecondLastName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _errors.Add("El segundo apellido es requerido");
                return;
            }
            if (value.Length < 2) _errors.Add("Debe tener al menos 2 caracteres");
            if (value.Length > 100) _errors.Add("No puede superar los 100 caracteres");
            if (value.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidatePhoneNumber(int value)
        {
            if (value <= 0) { _errors.Add("El teléfono es requerido"); return; }
            var len = value.ToString().Length;
            if (len < 7 || len > 12) _errors.Add("El teléfono debe tener entre 7 y 12 dígitos");
        }

        private void ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _errors.Add("El email es requerido");
                return;
            }
            if (email.Length > 100) _errors.Add("El email no puede superar los 100 caracteres");
            if (!email.Contains('@') || !email.Contains('.')) _errors.Add("Formato de email inválido");
            if (email.Any(c => ProhibitedChars.Contains(c))) _errors.Add("El email contiene caracteres no permitidos");
        }

        private void ValidateDocumentNumber(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _errors.Add("El número de documento es requerido");
                return;
            }
            if (value.Length < 5) _errors.Add("Debe tener al menos 5 caracteres");
            if (value.Length > 50) _errors.Add("No puede superar los 50 caracteres");
            if (value.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidateAddress(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _errors.Add("La dirección es requerida");
                return;
            }
            if (value.Length < 5) _errors.Add("Debe tener al menos 5 caracteres");
            if (value.Length > 200) _errors.Add("No puede superar los 200 caracteres");
            if (value.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidateBaseSalary(decimal? value)
        {
            if (value is null) { _errors.Add("El salario base es requerido"); return; }
            if (value < 0) _errors.Add("El salario base no puede ser negativo");
            if (value > 1_000_000) _errors.Add("El salario base es excesivo");
        }
    }
}
