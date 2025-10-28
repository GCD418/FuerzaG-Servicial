using UserAccountService.Domain.Ports;

namespace UserAccountService.Domain.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    }

    public bool VerifyPassword(string password, string hashedPassword)
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

    public string GenerateRandomPassword(int length = 8)
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string symbols = "!@#$%&*";
            
        const string allChars = lowercase + uppercase + digits + symbols;
            
        var random = new Random();
        var password = new char[length];
            
        password[0] = lowercase[random.Next(lowercase.Length)];
        password[1] = uppercase[random.Next(uppercase.Length)];
        password[2] = digits[random.Next(digits.Length)];
        password[3] = symbols[random.Next(symbols.Length)];
            
        for (int i = 4; i < length; i++)
        {
            password[i] = allChars[random.Next(allChars.Length)];
        }
            
        return new string(password.OrderBy(x => random.Next()).ToArray());
    }}