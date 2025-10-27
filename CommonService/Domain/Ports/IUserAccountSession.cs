namespace CommonService.Domain.Ports;

public interface IUserAccountSession
{
    
    public int Id { get; set; } 

    public string Name { get; set; }

    public string FirstLastName { get; set; }

    public string? SecondLastName { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public string Role { get; set; }

    public string FullName => $"{FirstLastName} {SecondLastName} {Name}";
}