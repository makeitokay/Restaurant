using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public interface IPasswordManager
{
	(string hash, string salt) GetPasswordHash(string password);
	bool VerifyPassword(string password, string passwordHash, string passwordSalt);
}

public class PasswordManager : IPasswordManager
{
	private const int HashingIterationsCount = 350000;
	private static readonly HashAlgorithmName _hashingAlgorithm = HashAlgorithmName.SHA512;
	private const int HashingKeySize = 64;

	public (string hash, string salt) GetPasswordHash(string password)
	{
		var salt = RandomNumberGenerator.GetBytes(16);

		var hash = Rfc2898DeriveBytes.Pbkdf2(
			Encoding.UTF8.GetBytes(password),
			salt,
			HashingIterationsCount,
			_hashingAlgorithm,
			HashingKeySize);

		return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
	}

	public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
	{
		var salt = Convert.FromBase64String(passwordSalt);
		
		var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			HashingIterationsCount,
			_hashingAlgorithm,
			HashingKeySize);

		return hashToCompare.SequenceEqual(Convert.FromBase64String(passwordHash));
	}
}