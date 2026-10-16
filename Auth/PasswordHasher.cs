namespace api_proj.Auth;

/// <summary>
/// Hashes and verifies passwords using BCrypt (BCrypt.Net-Next), per the
/// RaceDay API Build Guide (Authentication & Role-Based Access, Part U):
/// "PasswordHash stores the output of BCrypt, never the real password."
/// </summary>
public class PasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string storedHash) => BCrypt.Net.BCrypt.Verify(password, storedHash);
}
