using System.Security.Cryptography;

namespace WebApiWeb.Services
{
    public static class Seguridad
    {
        private const int Iteraciones = 100_000;
        private const int TamanoSalt = 16;
        private const int TamanoHash = 32;

        public static string Hashear(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(TamanoSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
            return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string password, string almacenado)
        {
            var partes = almacenado.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out var iteraciones))
            {
                return false;
            }

            var salt = Convert.FromBase64String(partes[1]);
            var esperado = Convert.FromBase64String(partes[2]);
            var calculado = Rfc2898DeriveBytes.Pbkdf2(password, salt, iteraciones, HashAlgorithmName.SHA256, esperado.Length);

            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
    }
}
