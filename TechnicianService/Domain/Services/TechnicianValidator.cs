using CommonService.Domain.Services.Validations;
using TechnicianService.Domain.Entities;

namespace TechnicianService.Domain.Services
{
    public class TechnicianValidator : IValidator<Technician>
    {
        private readonly List<string> _errors = [];

        public Result Validate(Technician technician)
        {
            _errors.Clear();

            ValidateName(technician.Name);
            ValidateFirstLastName(technician.FirstLastName);
            ValidateSecondLastName(technician.SecondLastName);
            ValidatePhoneNumber(technician.PhoneNumber);
            ValidateEmail(technician.Email);
            ValidateDocumentNumber(technician.DocumentNumber);
            ValidateAddress(technician.Address);
            ValidateBaseSalary(technician.BaseSalary);

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

        private void ValidateFirstLastName(string? firstLastName)
        {
            if (string.IsNullOrWhiteSpace(firstLastName))
            {
                _errors.Add("El primer apellido es requerido");
                return;
            }
            if (firstLastName.Length < 2) _errors.Add("Debe tener al menos 2 caracteres");
            if (firstLastName.Length > 100) _errors.Add("No puede superar los 100 caracteres");
            if (firstLastName.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidateSecondLastName(string? secontLastName)
        {
            if (string.IsNullOrWhiteSpace(secontLastName))
            {
                return;
            }
            if (secontLastName.Length < 2) _errors.Add("Debe tener al menos 2 caracteres");
            if (secontLastName.Length > 100) _errors.Add("No puede superar los 100 caracteres");
            if (secontLastName.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidatePhoneNumber(int phoneNumber)
        {
            if (phoneNumber <= 0) { _errors.Add("El teléfono es requerido"); return; }
            var len = phoneNumber.ToString().Length;
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

        private void ValidateDocumentNumber(string? documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                _errors.Add("El número de documento es requerido");
                return;
            }
            if (documentNumber.Length < 5) _errors.Add("Debe tener al menos 5 caracteres");
            if (documentNumber.Length > 50) _errors.Add("No puede superar los 50 caracteres");
            if (documentNumber.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidateAddress(string? address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                _errors.Add("La dirección es requerida");
                return;
            }
            if (address.Length < 5) _errors.Add("Debe tener al menos 5 caracteres");
            if (address.Length > 200) _errors.Add("No puede superar los 200 caracteres");
            if (address.Any(c => ProhibitedChars.Contains(c))) _errors.Add("Contiene caracteres no permitidos");
        }

        private void ValidateBaseSalary(decimal? baseSalary)
        {
            if (baseSalary is null) { _errors.Add("El salario base es requerido"); return; }
            if (baseSalary < 0) _errors.Add("El salario base no puede ser negativo");
            if (baseSalary > 1_000_000) _errors.Add("El salario base es excesivo");
        }

        private void ValidateDocumentExtension(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            if (value.Length != 2)
            {
                _errors.Add("El complemento debe tener exactamente 2 caracteres (ej. 1A).");
                return;
            }

            char first = value[0];
            char second = value[1];

            if (first < '1' || first > '9')
            {
                _errors.Add("El primer carácter del complemento debe ser un dígito entre 1 y 9.");
            }

            if (second < 'A' || second > 'Z')
            {
                _errors.Add("El segundo carácter del complemento debe ser una letra mayúscula A-Z.");
            }
        }
    }
}
