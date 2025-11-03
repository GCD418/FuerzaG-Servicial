namespace UserAccountService.Domain.Entities;


public class UserAccount
{
    public int Id { get; set; } 

    public string Name { get; set; } = string.Empty;

    public string FirstLastName { get; set; } = string.Empty;

    public string? SecondLastName { get; set; }

    public int PhoneNumber { get; set; }

    public string Email { get; set; } = string.Empty;

    public string DocumentNumber { get; set; } = string.Empty;
    
    public string DocumentExtension { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public int? ModifiedByUserId { get; set; }

    public bool IsFirstLogin { get; set; } = false;
    public string FullName => $"{FirstLastName} {(SecondLastName ?? string.Empty)} {Name}";

    public string FullDocumentNumber => $"{DocumentNumber}-{DocumentExtension}";
}