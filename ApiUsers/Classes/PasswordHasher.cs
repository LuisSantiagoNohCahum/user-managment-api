using System.Security.Cryptography;

namespace ApiUsers.Classes
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generar un salt aleatorio
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Crear la contraseña hash con el salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20); // Tamaño del hash es 20 bytes

            // Combinar salt + hash
            byte[] hashBytes = new byte[36]; // 16 (salt) + 20 (hash)
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convertir a string Base64 para almacenamiento
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }

        public static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Convertir la contraseña almacenada de Base64 a bytes
            byte[] hashBytes = Convert.FromBase64String(storedPassword);

            // Extraer salt desde el hash almacenado
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Generar hash con el salt y verificar
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Comparar los hashes
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
