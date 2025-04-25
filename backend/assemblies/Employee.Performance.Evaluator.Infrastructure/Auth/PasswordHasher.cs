using System.Security.Cryptography;
using Employee.Performance.Evaluator.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Employee.Performance.Evaluator.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var key = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: KeySize
        );

        var hash = new byte[1 + SaltSize + KeySize];
        hash[0] = 0x01;
        Buffer.BlockCopy(salt, 0, hash, 1, SaltSize);
        Buffer.BlockCopy(key, 0, hash, 1 + SaltSize, KeySize);

        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var decoded = Convert.FromBase64String(hashedPassword);

        if (decoded[0] != 0x01)
            throw new FormatException("Unknown hash version");

        var salt = new byte[SaltSize];
        var storedKey = new byte[KeySize];
        Buffer.BlockCopy(decoded, 1, salt, 0, SaltSize);
        Buffer.BlockCopy(decoded, 1 + SaltSize, storedKey, 0, KeySize);

        var derivedKey = KeyDerivation.Pbkdf2(
            password: providedPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: KeySize
        );

        return CryptographicOperations.FixedTimeEquals(derivedKey, storedKey);
    }
}